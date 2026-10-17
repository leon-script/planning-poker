using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Persistence;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class RoomStore : IRoomStore
    {
        private readonly PlanningPokerDbContext _db;

        public RoomStore(PlanningPokerDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(
            Room room,
            CancellationToken cancellationToken = default)
        {
            await _db.Rooms.AddAsync(room, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Room?> GetAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            return _db.Rooms.FirstOrDefaultAsync(
                x => x.Id == roomId,
                cancellationToken);
        }

        public Task<Room?> GetByAccessCodeHashAsync(
            string codeHash,
            CancellationToken cancellationToken = default)
        {
            return _db.Rooms.FirstOrDefaultAsync(
                x => x.AccessCodeHash == codeHash,
                cancellationToken);
        }

        public async Task<RoomSnapshot?> GetSnapshotAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            var room = await _db.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == roomId,
                    cancellationToken);

            if (room is null)
                return null;

            var participants = await _db.Participants
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.JoinedAt)
                .ToArrayAsync(cancellationToken);

            Estimation? estimation = null;
            IReadOnlyList<Vote> votes = Array.Empty<Vote>();

            if (room.CurrentEstimationId is Guid estimationId)
            {
                estimation = await _db.Estimations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.RoomId == roomId &&
                             x.Id == estimationId,
                        cancellationToken);

                if (estimation is not null)
                {
                    votes = await _db.Votes
                        .AsNoTracking()
                        .Where(x => x.EstimationId == estimation.Id)
                        .ToArrayAsync(cancellationToken);
                }
            }

            return new RoomSnapshot(
                room,
                participants,
                estimation,
                votes);
        }

        public async Task DeleteAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            await _db.Rooms
                .Where(x => x.Id == roomId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
