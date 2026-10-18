namespace PlanningPoker.Core.States
{
    public sealed record ParticipantState(
        Guid Id,
        string Name,
        string Role,
        bool HasVoted);
}
