using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Interfaces
{
    public interface IRoomStore
    {
        Task CreateAsync(
            Room room,
            CancellationToken cancellationToken = default);

        Task<Room?> GetAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);

        Task<Room?> GetByAccessCodeHashAsync(
            string codeHash,
            CancellationToken cancellationToken = default);

        Task<RoomSnapshot?> GetSnapshotAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);
    }
}
