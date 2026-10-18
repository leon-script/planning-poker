using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Security;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Interfaces;

namespace PlanningPoker.Application.Services
{
    public sealed class VoteService : IVoteService
    {
        private readonly IVoteStore _voteStore;
        private readonly IRoomEventPublisher _events;

        public VoteService(
            IVoteStore voteStore,
            IRoomEventPublisher events)
        {
            _voteStore = voteStore;
            _events = events;
        }

        public async Task SetVoteAsync(
            Guid roomId,
            Guid estimationId,
            string participantToken,
            float value,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(participantToken);

            if (!float.IsFinite(value) || value < 0)
            {
                throw new ArgumentException(
                    "Vote value must be a non-negative finite number.",
                    nameof(value));
            }

            var estimation = await _voteStore.GetEstimationAsync(
                roomId, estimationId, cancellationToken);

            if (estimation is null)
            {
                throw new KeyNotFoundException("Estimation not found.");
            }

            var tokenHash = SessionTokenHasher.Hash(participantToken);

            var participant = await _voteStore.GetParticipantByTokenHashAsync(roomId, tokenHash, cancellationToken);

            if (participant is null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid participant token.");
            }

            var vote = await _voteStore.GetVoteAsync(
                estimationId, participant.Id, cancellationToken);

            if (vote is not null && vote.Value == value)
                return;

            var now = DateTimeOffset.UtcNow;

            if (vote is null)
            {
                vote = new Vote
                {
                    Id = Guid.NewGuid(),
                    EstimationId = estimationId,
                    ParticipantId = participant.Id,
                    Value = value,
                    CreatedAt = now,
                    UpdatedAt = now
                };
            }
            else
            {
                vote.Value = value;
                vote.UpdatedAt = now;
            }

            await _voteStore.SaveVoteAsync(vote, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);
        }
    }

}
