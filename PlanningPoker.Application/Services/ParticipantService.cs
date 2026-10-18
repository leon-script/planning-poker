using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Security;
using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Interfaces;
using System.Security.Cryptography;

namespace PlanningPoker.Application.Services
{
    public sealed class ParticipantService : IParticipantService
    {
        private readonly IParticipantStore _participantStore;
        private readonly IRoomStore _roomStore;
        private readonly IRoomEventPublisher _events;

        public ParticipantService(
            IParticipantStore participantStore,
            IRoomStore roomStore,
            IRoomEventPublisher events)
        {
            _participantStore = participantStore;
            _roomStore = roomStore;
            _events = events;
        }

        public async Task<JoinRoomResult> JoinAsync(
            string code,
            string name,
            string role,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(role);

            code = NormalizeCode(code);
            name = name.Trim();
            role = role.Trim();

            if (name.Length > 50 || role.Length > 50)
            {
                throw new ArgumentException("Name and role must not exceed 50 characters.");
            }

            var codeHash = SessionTokenHasher.Hash(code);
            var room = await _roomStore.GetByAccessCodeHashAsync(codeHash, cancellationToken);

            if (room is null)
            {
                throw new UnauthorizedAccessException("Invalid room code.");
            }

            var participant = await _participantStore.GetByNameAndRoleAsync(
                room.Id,
                name,
                role,
                cancellationToken);

            var participantToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            if (participant is null)
            {
                participant = new Participant
                {
                    Id = Guid.NewGuid(),
                    RoomId = room.Id,
                    Name = name,
                    Role = role,
                    TokenHash = SessionTokenHasher.Hash(participantToken),
                    JoinedAt = DateTimeOffset.UtcNow
                };

                await _participantStore.CreateAsync(participant, cancellationToken);
            }
            else
            {
                participant.TokenHash = SessionTokenHasher.Hash(participantToken);
                await _participantStore.UpdateAsync(participant, cancellationToken);
            }

            await _events.RoomUpdatedAsync(room.Id, cancellationToken);

            return new JoinRoomResult(
                room.Id,
                participant.Id,
                participantToken);
        }

        public async Task<ParticipantProfile> GetMeAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default)
        {
            var participant = await GetParticipantByTokenAsync(
                roomId,
                participantToken,
                cancellationToken);

            return new ParticipantProfile(
                participant.Id,
                participant.Name,
                participant.Role);
        }

        public async Task LeaveAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default)
        {
            var participant = await GetParticipantByTokenAsync(
                roomId,
                participantToken,
                cancellationToken);

            await _participantStore.DeleteAsync(roomId, participant.Id, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);
        }

        public async Task RemoveAsync(
            Guid roomId,
            Guid participantId,
            string roomCode,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(roomCode);

            var room = await _roomStore.GetAsync(
                roomId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Room not found.");

            var providedHash = SessionTokenHasher.Hash(NormalizeCode(roomCode));

            if (!VerifyHash(room.AccessCodeHash, providedHash))
            {
                throw new UnauthorizedAccessException("Invalid room code.");
            }

            var participant = await _participantStore.GetAsync(
                roomId,
                participantId,
                cancellationToken)
                ?? throw new KeyNotFoundException("Participant not found.");

            await _participantStore.DeleteAsync(roomId, participant.Id, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);
        }

        private async Task<Participant> GetParticipantByTokenAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
            {
                throw new UnauthorizedAccessException("Participant token is required.");
            }

            var tokenHash = SessionTokenHasher.Hash(participantToken);

            return await _participantStore.GetByTokenHashAsync(
                roomId,
                tokenHash,
                cancellationToken)
                ?? throw new UnauthorizedAccessException(
                    "Invalid participant token.");
        }

        private static string NormalizeCode(string code)
        {
            return code.Trim().ToUpperInvariant();
        }

        private static bool VerifyHash(
            string expectedHash,
            string providedHash)
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(expectedHash),
                Convert.FromHexString(providedHash));
        }
    }
}
