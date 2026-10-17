using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Persistence;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class VoteStore : IVoteStore
    {
        private readonly PlanningPokerDbContext _db;

        public VoteStore(PlanningPokerDbContext db)
        {
            _db = db;
        }

        public Task<Estimation?> GetEstimationAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            return _db.Estimations.FirstOrDefaultAsync(
                x => x.Id == estimationId &&
                     x.RoomId == roomId,
                cancellationToken);
        }

        public Task<Participant?> GetParticipantByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return _db.Participants.FirstOrDefaultAsync(
                x => x.RoomId == roomId &&
                     x.TokenHash == tokenHash,
                cancellationToken);
        }

        public Task<Vote?> GetVoteAsync(
            Guid estimationId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            return _db.Votes.FirstOrDefaultAsync(
                x => x.EstimationId == estimationId &&
                     x.ParticipantId == participantId,
                cancellationToken);
        }

        public async Task SaveVoteAsync(
            Vote vote,
            CancellationToken cancellationToken = default)
        {
            if (_db.Entry(vote).State == EntityState.Detached)
            {
                _db.Votes.Add(vote);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
