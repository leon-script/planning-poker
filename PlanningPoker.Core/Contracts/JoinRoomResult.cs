namespace PlanningPoker.Core.Contracts
{
    public sealed record JoinRoomResult(
        Guid RoomId,
        Guid ParticipantId,
        string ParticipantToken);
}
