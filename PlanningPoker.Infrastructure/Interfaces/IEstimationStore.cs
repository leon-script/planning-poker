using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.Interfaces
{
    public interface IEstimationStore
    {
        Task<IReadOnlyList<Estimation>> GetAllAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);

        Task<Estimation?> GetAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Vote>> GetVotesAsync(
            Guid estimationId,
            CancellationToken cancellationToken = default);

        Task CreateAndSetCurrentAsync(
            Room room,
            Estimation estimation,
            CancellationToken cancellationToken = default);

        Task SetCurrentAsync(
            Room room,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default);

        Task ClearVotesAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default);
    }
}
