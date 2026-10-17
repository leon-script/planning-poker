using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Persistence;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class EstimationStore : IEstimationStore
    {
        private readonly PlanningPokerDbContext _db;

        public EstimationStore(PlanningPokerDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Estimation>> GetAllAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Estimations
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Estimation?> GetAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            return _db.Estimations.FirstOrDefaultAsync(
                x => x.Id == estimationId &&
                     x.RoomId == roomId,
                cancellationToken);
        }

        public async Task<IReadOnlyList<Vote>> GetVotesAsync(
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Votes
                .AsNoTracking()
                .Where(x => x.EstimationId == estimationId)
                .ToArrayAsync(cancellationToken);
        }

        public async Task CreateAndSetCurrentAsync(
            Room room,
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            if (_db.Entry(room).State == EntityState.Detached)
            {
                _db.Rooms.Update(room);
            }

            await _db.Estimations.AddAsync(
                estimation,
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetCurrentAsync(
            Room room,
            CancellationToken cancellationToken = default)
        {
            if (_db.Entry(room).State == EntityState.Detached)
            {
                _db.Rooms.Update(room);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            if (_db.Entry(estimation).State == EntityState.Detached)
            {
                _db.Estimations.Update(estimation);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task ClearVotesAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _db.Database.BeginTransactionAsync(
                    cancellationToken);

            await _db.Votes
                .Where(x => x.EstimationId == estimation.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (_db.Entry(estimation).State == EntityState.Detached)
            {
                _db.Estimations.Update(estimation);
            }

            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
