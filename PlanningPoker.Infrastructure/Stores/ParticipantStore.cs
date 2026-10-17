using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Persistence;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class ParticipantStore : IParticipantStore
    {
        private readonly PlanningPokerDbContext _db;

        public ParticipantStore(PlanningPokerDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(
            Participant participant,
            CancellationToken cancellationToken = default)
        {
            await _db.Participants.AddAsync(
                participant,
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Participant?> GetAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            return _db.Participants.FirstOrDefaultAsync(
                x => x.Id == participantId &&
                     x.RoomId == roomId,
                cancellationToken);
        }

        public Task<Participant?> GetByNameAndRoleAsync(
            Guid roomId,
            string name,
            string role,
            CancellationToken cancellationToken = default)
        {
            return _db.Participants.FirstOrDefaultAsync(
                x => x.RoomId == roomId &&
                     x.Name == name &&
                     x.Role == role,
                cancellationToken);
        }

        public Task<Participant?> GetByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return _db.Participants.FirstOrDefaultAsync(
                x => x.RoomId == roomId &&
                     x.TokenHash == tokenHash,
                cancellationToken);
        }

        public async Task UpdateAsync(
            Participant participant,
            CancellationToken cancellationToken = default)
        {
            if (_db.Entry(participant).State == EntityState.Detached)
            {
                _db.Participants.Update(participant);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var exists = await _db.Participants.AnyAsync(
                x => x.Id == participantId &&
                     x.RoomId == roomId,
                cancellationToken);

            if (!exists)
                return;

            await using var transaction =
                await _db.Database.BeginTransactionAsync(
                    cancellationToken);

            await _db.Votes
                .Where(x => x.ParticipantId == participantId)
                .ExecuteDeleteAsync(cancellationToken);

            await _db.Participants
                .Where(x => x.Id == participantId &&
                            x.RoomId == roomId)
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
