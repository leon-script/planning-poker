namespace PlanningPoker.Core.Contracts
{
    public sealed record ParticipantState(
        Guid Id,
        string Name,
        string Role,
        bool HasVoted);
}
