namespace PlanningPoker.Core.Contracts
{
    public sealed record ParticipantProfile(
        Guid Id,
        string Name,
        string Role);
}
