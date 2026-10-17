namespace PlanningPoker.Core.Contracts
{
    public sealed record VoteState(
        Guid ParticipantId,
        float? Value);
}
