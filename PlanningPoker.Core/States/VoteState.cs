namespace PlanningPoker.Core.States
{
    public sealed record VoteState(
        Guid ParticipantId,
        float? Value);
}
