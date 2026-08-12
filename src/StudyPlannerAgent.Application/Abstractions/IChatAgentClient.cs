using StudyPlannerAgent.Application.Chat;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IChatAgentClient
{
    Task<Result<ChatResponse>> SendMessageAsync(Guid userId, ChatRequest request, CancellationToken cancellationToken);
}
