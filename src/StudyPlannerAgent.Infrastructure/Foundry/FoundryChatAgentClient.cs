using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Chat;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Infrastructure.Foundry;

public sealed class FoundryChatAgentClient : IChatAgentClient
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(700);
    private static readonly TimeSpan RunTimeout = TimeSpan.FromSeconds(60);

    private readonly PersistentAgentsClient _client;
    private readonly FoundryAgentOptions _options;

    public FoundryChatAgentClient(FoundryAgentOptions options)
    {
        _options = options;
        _client = new PersistentAgentsClient(options.Endpoint, new DefaultAzureCredential());
    }

    public async Task<Result<ChatResponse>> SendMessageAsync(Guid userId, ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var thread = string.IsNullOrWhiteSpace(request.ThreadId)
                ? await _client.Threads.CreateThreadAsync(cancellationToken: cancellationToken)
                : await _client.Threads.GetThreadAsync(request.ThreadId, cancellationToken);

            await _client.Messages.CreateMessageAsync(
                thread.Value.Id,
                MessageRole.User,
                request.Message,
                cancellationToken: cancellationToken);

            var run = await _client.Runs.CreateRunAsync(
                thread.Value.Id,
                _options.AgentId,
                additionalInstructions: $"Current authenticated userId: {userId}. Use this userId when calling MCP tools.",
                cancellationToken: cancellationToken);

            var completedRun = await WaitForRunAsync(thread.Value.Id, run.Value.Id, cancellationToken);

            if (completedRun.IsFailure)
                return Result<ChatResponse>.Failure(completedRun.Error);

            var response = await GetLastAgentMessageAsync(thread.Value.Id, cancellationToken);

            if (response.IsFailure)
                return Result<ChatResponse>.Failure(response.Error);

            return Result<ChatResponse>.Success(new ChatResponse(thread.Value.Id, response.Value));
        }
        catch (RequestFailedException exception)
        {
            return Result<ChatResponse>.Failure(new Error(
                "Foundry.RequestFailed",
                $"Foundry request failed: {exception.Message}"));
        }
        catch (Exception exception)
        {
            return Result<ChatResponse>.Failure(new Error(
                "Foundry.UnexpectedError",
                $"Unexpected Foundry error: {exception.Message}"));
        }
    }

    private async Task<Result> WaitForRunAsync(string threadId, string runId, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - startedAt < RunTimeout)
        {
            var run = await _client.Runs.GetRunAsync(threadId, runId, cancellationToken);

            if (run.Value.Status == RunStatus.Completed)
                return Result.Success();

            if (run.Value.Status == RunStatus.Failed
                || run.Value.Status == RunStatus.Cancelled
                || run.Value.Status == RunStatus.Expired)
            {
                return Result.Failure(new Error(
                    "Foundry.RunFailed",
                    run.Value.LastError?.Message ?? $"Foundry run finished with status {run.Value.Status}."));
            }

            await Task.Delay(PollInterval, cancellationToken);
        }

        return Result.Failure(new Error("Foundry.RunTimeout", "Foundry run did not complete before timeout."));
    }

    private async Task<Result<string>> GetLastAgentMessageAsync(string threadId, CancellationToken cancellationToken)
    {
        var messages = _client.Messages.GetMessagesAsync(threadId, order: ListSortOrder.Descending, cancellationToken: cancellationToken);

        await foreach (var message in messages.WithCancellation(cancellationToken))
        {
            if (message.Role != MessageRole.Agent)
                continue;

            var textParts = message.ContentItems
                .OfType<MessageTextContent>()
                .Select(content => content.Text)
                .Where(text => !string.IsNullOrWhiteSpace(text));

            var text = string.Join(Environment.NewLine, textParts);

            if (!string.IsNullOrWhiteSpace(text))
                return Result<string>.Success(text);
        }

        return Result<string>.Failure(new Error("Foundry.EmptyResponse", "Foundry agent did not return a text response."));
    }
}
