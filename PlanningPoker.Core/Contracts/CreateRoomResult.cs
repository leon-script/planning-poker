namespace PlanningPoker.Core.Contracts
{
    public sealed record CreateRoomResult(
        Guid RoomId,
        string Code);
}
