using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Stores
{
    public sealed class ParticipantStore : IParticipantStore
    {
        private readonly PlanningPokerDbContext _database;
        private readonly IMapper _mapper;

        public ParticipantStore(
            PlanningPokerDbContext database,
            IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
        }

        public async Task CreateAsync(
            Participant participant,
            CancellationToken cancellationToken = default)
        {
            var dao = _mapper.Map<ParticipantDao>(participant);

            await _database.Participants.AddAsync(dao, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task<Participant?> GetAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants
                .FirstOrDefaultAsync(
                    x => x.Id == participantId &&
                         x.RoomId == roomId,
                    cancellationToken);

            return _mapper.Map<Participant?>(dao);
        }

        public async Task<Participant?> GetByNameAndRoleAsync(
            Guid roomId,
            string name,
            string role,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants
                .FirstOrDefaultAsync(
                    x => x.RoomId == roomId &&
                         x.Name == name &&
                         x.Role == role,
                    cancellationToken);

            return _mapper.Map<Participant?>(dao);
        }

        public async Task<Participant?> GetByTokenHashAsync(
            Guid roomId,
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants
                .FirstOrDefaultAsync(
                    x => x.RoomId == roomId &&
                         x.TokenHash == tokenHash,
                    cancellationToken);

            return _mapper.Map<Participant?>(dao);
        }

        public async Task UpdateAsync(
            Participant participant,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants.FindAsync(
                new object[] { participant.Id },
                cancellationToken)
                ?? throw new KeyNotFoundException("Participant not found.");

            if (dao.RoomId != participant.RoomId)
            {
                throw new KeyNotFoundException("Participant not found.");
            }

            _database.Entry(dao).CurrentValues.SetValues(participant);

            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid roomId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var dao = await _database.Participants.FindAsync(
                new object[] { participantId },
                cancellationToken);

            if (dao is null || dao.RoomId != roomId)
            {
                return;
            }

            await using var transaction = await _database.Database.BeginTransactionAsync(cancellationToken);

            await _database.Votes
                .Where(x => x.ParticipantId == participantId)
                .ExecuteDeleteAsync(cancellationToken);

            _database.Participants.Remove(dao);

            await _database.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
