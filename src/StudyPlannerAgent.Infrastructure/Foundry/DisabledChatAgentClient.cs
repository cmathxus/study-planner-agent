using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Chat;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Infrastructure.Foundry;

public sealed class DisabledChatAgentClient : IChatAgentClient
{
    public Task<Result<ChatResponse>> SendMessageAsync(Guid userId, ChatRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<ChatResponse>.Failure(new Error(
            "Foundry.NotConfigured",
            "Foundry chat is not configured. Set Foundry__Endpoint and Foundry__AgentId.")));
    }
}
