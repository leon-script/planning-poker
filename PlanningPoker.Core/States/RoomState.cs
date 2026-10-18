namespace PlanningPoker.Core.States
{
    public sealed record RoomState(
        Guid Id,
        IReadOnlyList<ParticipantState> Participants,
        EstimationState? CurrentEstimation);
}
