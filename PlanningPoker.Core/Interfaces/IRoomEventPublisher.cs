namespace PlanningPoker.Core.Interfaces
{
    public interface IRoomEventPublisher
    {
        Task RoomUpdatedAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);

        Task RoomDeletedAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);
    }
}
