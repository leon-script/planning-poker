using PlanningPoker.Core.Entities;
using PlanningPoker.Core.States;

namespace PlanningPoker.Application.Interfaces
{
    public interface IEstimationService
    {
        Task<IReadOnlyList<Estimation>> GetAllAsync(
            Guid roomId,
            CancellationToken cancellationToken = default);

        Task<EstimationState?> GetStateAsync(
            Guid roomId,
            Guid estimationId,
            string? participantToken = null,
            CancellationToken cancellationToken = default);

        Task<Estimation> CreateAsync(
            Guid roomId,
            string title,
            string? description,
            CancellationToken cancellationToken = default);

        Task<Estimation> UpdateDetailsAsync(
            Guid roomId,
            Guid estimationId,
            string title,
            string? description,
            CancellationToken cancellationToken = default);

        Task<Estimation> SetVisibilityAsync(
            Guid roomId,
            Guid estimationId,
            bool isRevealed,
            CancellationToken cancellationToken = default);

        Task<Estimation> ClearVotesAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default);

        Task<Estimation> SetResultAsync(
            Guid roomId,
            Guid estimationId,
            float? finalValue,
            CancellationToken cancellationToken = default);

        Task<Estimation> ActivateAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default);
    }
}
