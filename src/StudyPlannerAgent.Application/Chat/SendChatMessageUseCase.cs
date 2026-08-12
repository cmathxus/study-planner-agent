using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.Chat;

public sealed class SendChatMessageUseCase
{
    private readonly IChatAgentClient _chatAgentClient;

    public SendChatMessageUseCase(IChatAgentClient chatAgentClient)
    {
        _chatAgentClient = chatAgentClient;
    }

    public async Task<Result<ChatResponse>> ExecuteAsync(Guid userId, ChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return Result<ChatResponse>.Failure(new Error("Chat.EmptyMessage", "Chat message cannot be empty."));

        return await _chatAgentClient.SendMessageAsync(userId, request, cancellationToken);
    }
}
