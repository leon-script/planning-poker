using PlanningPoker.Core.Contracts;

namespace PlanningPoker.Application.Interfaces
{
    public interface IParticipantService
    {
        Task<JoinRoomResult> JoinAsync(
            string code,
            string name,
            string role,
            CancellationToken cancellationToken = default);

        Task<ParticipantProfile> GetMeAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default);

        Task LeaveAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(
            Guid roomId,
            Guid participantId,
            string roomCode,
            CancellationToken cancellationToken = default);
    }
}
