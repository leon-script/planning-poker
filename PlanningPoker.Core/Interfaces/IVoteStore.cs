using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Interfaces
{
    public interface IVoteStore
    {
        Task<Estimation?> GetEstimationAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default);

        Task<Participant?> GetParticipantByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<Vote?> GetVoteAsync(
            Guid estimationId,
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task SaveVoteAsync(
            Vote vote,
            CancellationToken cancellationToken = default);
    }
}
