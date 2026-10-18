using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Snapshots;
using PlanningPoker.Infrastructure.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class RoomStore : IRoomStore
    {
        private readonly PlanningPokerDbContext _database;
        private readonly IMapper _mapper;

        public RoomStore(
            PlanningPokerDbContext database,
            IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
        }

        public async Task CreateAsync(
            Room room,
            CancellationToken cancellationToken = default)
        {
            var dao = _mapper.Map<RoomDao>(room);

            await _database.Rooms.AddAsync(dao, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task<Room?> GetAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Rooms.FindAsync(
                new object[] { roomId },
                cancellationToken);

            return _mapper.Map<Room?>(dao);
        }

        public async Task<Room?> GetByAccessCodeHashAsync(
            string codeHash,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Rooms
                .FirstOrDefaultAsync(
                    x => x.AccessCodeHash == codeHash,
                    cancellationToken);

            return _mapper.Map<Room?>(dao);
        }

        public async Task<RoomSnapshot?> GetSnapshotAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            var roomDao = await _database.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == roomId,
                    cancellationToken);

            if (roomDao is null)
            {
                return null;
            }

            var participantDaos = await _database.Participants
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.JoinedAt)
                .ToArrayAsync(cancellationToken);

            EstimationDao? estimationDao = null;
            IReadOnlyList<VoteDao> voteDaos = [];

            if (roomDao.CurrentEstimationId is Guid estimationId)
            {
                estimationDao = await _database.Estimations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.RoomId == roomId &&
                             x.Id == estimationId,
                        cancellationToken);

                if (estimationDao is not null)
                {
                    voteDaos = await _database.Votes
                        .AsNoTracking()
                        .Where(x => x.EstimationId == estimationDao.Id)
                        .ToArrayAsync(cancellationToken);
                }
            }

            var room = _mapper.Map<Room>(roomDao);
            var participants = _mapper.Map<IReadOnlyList<Participant>>(participantDaos);
            var estimation = estimationDao is null ? null : _mapper.Map<Estimation>(estimationDao);
            var votes = _mapper.Map<IReadOnlyList<Vote>>(voteDaos);

            return new RoomSnapshot(room, participants, estimation, votes);
        }

        public async Task DeleteAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Rooms.FindAsync(
                new object[] { roomId },
                cancellationToken);

            if (dao is null)
            {
                return;
            }

            _database.Rooms.Remove(dao);
            await _database.SaveChangesAsync(cancellationToken);
        }
    }
}
