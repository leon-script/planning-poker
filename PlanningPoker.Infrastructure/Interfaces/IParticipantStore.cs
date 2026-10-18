using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.Interfaces
{
    public interface IParticipantStore
    {
        Task CreateAsync(
            Participant participant,
            CancellationToken cancellationToken = default);

        Task<Participant?> GetAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task<Participant?> GetByNameAndRoleAsync(
            Guid roomId,
            string name,
            string role,
            CancellationToken cancellationToken = default);

        Task<Participant?> GetByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Participant participant,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default);
    }
}
