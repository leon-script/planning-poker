using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Security;
using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PlanningPoker.Application.Services
{
    public sealed class RoomService : IRoomService
    {
        private const string RoomCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        private readonly IRoomStore _roomStore;
        private readonly IRoomEventPublisher _events;

        public RoomService(
            IRoomStore roomStore,
            IRoomEventPublisher events)
        {
            _roomStore = roomStore;
            _events = events;
        }

        public async Task<CreateRoomResult> CreateAsync(
            CancellationToken cancellationToken = default)
        {
            var code = RandomNumberGenerator.GetString(RoomCodeAlphabet, 8);

            var now = DateTimeOffset.UtcNow;

            var room = new Room
            {
                Id = Guid.NewGuid(),
                AccessCodeHash = SessionTokenHasher.Hash(code),
                CreatedAt = now,
                LastActivityAt = now
            };

            await _roomStore.CreateAsync(room, cancellationToken);

            return new CreateRoomResult(room.Id, code);
        }

        public async Task<RoomState?> GetStateAsync(
            Guid roomId,
            string participantToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
            {
                throw new UnauthorizedAccessException("Participant token is required.");
            }

            var snapshot = await _roomStore.GetSnapshotAsync(roomId, cancellationToken);

            if (snapshot is null)
                return null;

            var tokenHash = SessionTokenHasher.Hash(participantToken);

            var currentParticipant = snapshot.Participants
                .FirstOrDefault(p => p.TokenHash == tokenHash);

            if (currentParticipant is null)
            {
                throw new UnauthorizedAccessException("Invalid participant token.");
            }

            var estimation = snapshot.CurrentEstimation;

            var participantIds = snapshot.Participants
                .Select(p => p.Id)
                .ToHashSet();

            var currentVotes = estimation is null
                ? Array.Empty<Vote>()
                : snapshot.Votes
                    .Where(v =>
                        v.EstimationId == estimation.Id &&
                        participantIds.Contains(v.ParticipantId))
                    .ToArray();

            var votedParticipantIds = currentVotes
                .Select(v => v.ParticipantId)
                .ToHashSet();

            var participants = snapshot.Participants
                .Select(p => new ParticipantState(
                    p.Id,
                    p.Name,
                    p.Role,
                    votedParticipantIds.Contains(p.Id)))
                .ToArray();

            EstimationState? estimationState = null;

            if (estimation is not null)
            {
                var votes = currentVotes
                    .Select(v => new VoteState(
                        v.ParticipantId,
                        estimation.IsRevealed ||
                        v.ParticipantId == currentParticipant.Id
                            ? v.Value
                            : null))
                    .ToArray();

                estimationState = new EstimationState(
                    estimation.Id,
                    estimation.Title,
                    estimation.Description,
                    estimation.IsRevealed,
                    estimation.FinalValue,
                    votes);
            }

            return new RoomState(
                snapshot.Room.Id,
                participants,
                estimationState);
        }

        public async Task DeleteAsync(
            Guid roomId,
            string roomCode,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(roomCode);

            var room = await _roomStore.GetAsync(roomId, cancellationToken);

            if (room is null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var normalizedCode = roomCode.Trim().ToUpperInvariant();
            var providedHash = SessionTokenHasher.Hash(normalizedCode);

            var isValid = CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(room.AccessCodeHash),
                Convert.FromHexString(providedHash));

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid room code.");
            }

            await _roomStore.DeleteAsync(roomId, cancellationToken);
            await _events.RoomDeletedAsync(roomId, cancellationToken);
        }
    }

}
