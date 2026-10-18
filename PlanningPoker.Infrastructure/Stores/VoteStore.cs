using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class VoteStore : IVoteStore
    {
        private readonly PlanningPokerDbContext _database;
        private readonly IMapper _mapper;

        public VoteStore(
            PlanningPokerDbContext database,
            IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
        }

        public async Task<Estimation?> GetEstimationAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Estimations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == estimationId &&
                         x.RoomId == roomId,
                    cancellationToken);

            return _mapper.Map<Estimation?>(dao);
        }

        public async Task<Participant?> GetParticipantByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.RoomId == roomId && 
                         x.TokenHash == tokenHash,
                    cancellationToken);

            return _mapper.Map<Participant?>(dao);
        }

        public async Task<Vote?> GetVoteAsync(
            Guid estimationId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Votes
                .FirstOrDefaultAsync(
                    x => x.EstimationId == estimationId &&
                         x.ParticipantId == participantId,
                    cancellationToken);

            return _mapper.Map<Vote?>(dao);
        }

        public async Task SaveVoteAsync(
            Vote vote,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Votes.FindAsync(
                new object[] { vote.Id },
                cancellationToken);

            if (dao is null)
            {
                var newDao = _mapper.Map<VoteDao>(vote);
                await _database.Votes.AddAsync(newDao, cancellationToken);
            }
            else
            {
                if (dao.EstimationId != vote.EstimationId || dao.ParticipantId != vote.ParticipantId)
                {
                    throw new InvalidOperationException("Vote belongs to another estimation or participant.");
                }

                _database.Entry(dao).CurrentValues.SetValues(vote);
            }

            await _database.SaveChangesAsync(cancellationToken);
        }
    }
}
