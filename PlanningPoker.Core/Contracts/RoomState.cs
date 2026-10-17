namespace PlanningPoker.Core.Contracts
{
    public sealed record RoomState(
        Guid Id,
        IReadOnlyList<ParticipantState> Participants,
        EstimationState? CurrentEstimation);
}
