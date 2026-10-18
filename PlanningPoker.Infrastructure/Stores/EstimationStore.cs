using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class EstimationStore : IEstimationStore
    {
        private readonly PlanningPokerDbContext _database;
        private readonly IMapper _mapper;

        public EstimationStore(
            PlanningPokerDbContext database,
            IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<Estimation>> GetAllAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            var daos = await _database.Estimations
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<Estimation>>(daos);
        }

        public async Task<Estimation?> GetAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Estimations
                .FirstOrDefaultAsync(
                    x => x.Id == estimationId &&
                         x.RoomId == roomId,
                    cancellationToken);

            return _mapper.Map<Estimation?>(dao);
        }

        public async Task<IReadOnlyList<Vote>> GetVotesAsync(
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            var daos = await _database.Votes
                .AsNoTracking()
                .Where(x => x.EstimationId == estimationId)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<Vote>>(daos);
        }

        public async Task CreateAndSetCurrentAsync(
            Room room,
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            if (estimation.RoomId != room.Id)
            {
                throw new ArgumentException("Estimation belongs to another room.");
            }

            var roomDao = await _database.Rooms.FindAsync(
                new object[] { room.Id },
                cancellationToken)
                ?? throw new KeyNotFoundException("Room not found.");

            _database.Entry(roomDao).CurrentValues.SetValues(room);

            var estimationDao = _mapper.Map<EstimationDao>(estimation);

            await _database.Estimations.AddAsync(estimationDao, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task SetCurrentAsync(
            Room room,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Rooms.FindAsync(
                new object[] { room.Id },
                cancellationToken)
                ?? throw new KeyNotFoundException("Room not found.");

            _database.Entry(dao).CurrentValues.SetValues(room);

            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Estimations.FindAsync(
                new object[] { estimation.Id },
                cancellationToken)
                ?? throw new KeyNotFoundException("Estimation not found.");

            if (dao.RoomId != estimation.RoomId)
            {
                throw new KeyNotFoundException("Estimation not found.");
            }

            _database.Entry(dao).CurrentValues.SetValues(estimation);

            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task ClearVotesAsync(
            Estimation estimation,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Estimations.FindAsync(
                new object[] { estimation.Id },
                cancellationToken)
                ?? throw new KeyNotFoundException("Estimation not found.");

            if (dao.RoomId != estimation.RoomId)
            {
                throw new KeyNotFoundException("Estimation not found.");
            }

            await using var transaction = await _database.Database.BeginTransactionAsync(cancellationToken);

            await _database.Votes
                .Where(x => x.EstimationId == estimation.Id)
                .ExecuteDeleteAsync(cancellationToken);

            _database.Entry(dao).CurrentValues.SetValues(estimation);

            await _database.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
