using PlanningPoker.Core.Contracts;

namespace PlanningPoker.Application.Interfaces
{
    public interface IRoomService
    {
        Task<CreateRoomResult> CreateAsync(
            CancellationToken cancellationToken = default);

        Task<RoomState?> GetStateAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid roomId,
            string roomCode,
            CancellationToken cancellationToken = default);
    }
}
