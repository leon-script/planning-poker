using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Security;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Core.States;
using PlanningPoker.Infrastructure.Interfaces;

namespace PlanningPoker.Application.Services
{
    public sealed class EstimationService : IEstimationService
    {
        private readonly IEstimationStore _estimationStore;
        private readonly IRoomStore _roomStore;
        private readonly IVoteStore _voteStore;
        private readonly IRoomEventPublisher _events;

        public EstimationService(
            IEstimationStore estimationStore,
            IRoomStore roomStore,
            IVoteStore voteStore,
            IRoomEventPublisher events)
        {
            _estimationStore = estimationStore;
            _roomStore = roomStore;
            _voteStore = voteStore;
            _events = events;
        }

        public async Task<IReadOnlyList<Estimation>> GetAllAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            await GetRoomAsync(roomId, cancellationToken);

            return await _estimationStore.GetAllAsync(roomId, cancellationToken);
        }

        public async Task<EstimationState?> GetStateAsync(
            Guid roomId,
            Guid estimationId,
            string? participantToken = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
            {
                throw new UnauthorizedAccessException("Participant token is required.");
            }

            var tokenHash = SessionTokenHasher.Hash(participantToken);
            var participant = await _voteStore.GetParticipantByTokenHashAsync(roomId, tokenHash, cancellationToken);

            if (participant is null)
            {
                throw new UnauthorizedAccessException("Invalid participant token.");
            }

            var estimation = await _estimationStore.GetAsync(roomId, estimationId, cancellationToken);

            if (estimation is null)
            {
                return null;
            }

            var votes = await _estimationStore.GetVotesAsync(estimationId, cancellationToken);

            var voteStates = votes
                .Select(v => new VoteState(
                    v.ParticipantId,
                    estimation.IsRevealed || v.ParticipantId == participant.Id
                        ? v.Value
                        : null))
                .ToArray();

            return new EstimationState(
                estimation.Id,
                estimation.Title,
                estimation.Description,
                estimation.IsRevealed,
                estimation.FinalValue,
                voteStates);
        }

        public async Task<Estimation> CreateAsync(
            Guid roomId,
            string title,
            string? description,
            CancellationToken cancellationToken = default)
        {
            ValidateDetails(title, description);

            var room = await GetRoomAsync(roomId, cancellationToken);
            var now = DateTimeOffset.UtcNow;

            var estimation = new Estimation
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                Title = title.Trim(),
                Description = description?.Trim(),
                IsRevealed = false,
                FinalValue = null,
                CreatedAt = now
            };

            room.CurrentEstimationId = estimation.Id;
            room.LastActivityAt = now;

            await _estimationStore.CreateAndSetCurrentAsync(room, estimation, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        public async Task<Estimation> UpdateDetailsAsync(
            Guid roomId,
            Guid estimationId,
            string title,
            string? description,
            CancellationToken cancellationToken = default)
        {
            ValidateDetails(title, description);

            var estimation = await GetEstimationAsync(roomId, estimationId, cancellationToken);
            var newTitle = title.Trim();
            var newDescription = description?.Trim();

            if (estimation.Title == newTitle && estimation.Description == newDescription)
            {
                return estimation;
            }

            estimation.Title = newTitle;
            estimation.Description = newDescription;

            await _estimationStore.UpdateAsync(estimation, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        public async Task<Estimation> SetVisibilityAsync(
            Guid roomId,
            Guid estimationId,
            bool isRevealed,
            CancellationToken cancellationToken = default)
        {
            var estimation = await GetEstimationAsync(roomId, estimationId, cancellationToken);

            if (estimation.IsRevealed == isRevealed)
            {
                return estimation;
            }

            estimation.IsRevealed = isRevealed;
            await _estimationStore.UpdateAsync(estimation, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        public async Task<Estimation> ClearVotesAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            var estimation = await GetEstimationAsync(roomId, estimationId, cancellationToken);

            estimation.IsRevealed = false;
            estimation.FinalValue = null;

            await _estimationStore.ClearVotesAsync(estimation, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        public async Task<Estimation> SetResultAsync(
            Guid roomId,
            Guid estimationId,
            float? finalValue,
            CancellationToken cancellationToken = default)
        {
            if (finalValue.HasValue && (!float.IsFinite(finalValue.Value) || finalValue.Value < 0))
            {
                throw new ArgumentException("Final value must be a non-negative finite number.", nameof(finalValue));
            }

            var estimation = await GetEstimationAsync(roomId, estimationId, cancellationToken);

            if (estimation.FinalValue == finalValue)
            {
                return estimation;
            }

            estimation.FinalValue = finalValue;
            await _estimationStore.UpdateAsync(estimation, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        public async Task<Estimation> ActivateAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken = default)
        {
            var room = await GetRoomAsync(roomId, cancellationToken);
            var estimation = await GetEstimationAsync(roomId, estimationId, cancellationToken);

            if (room.CurrentEstimationId == estimationId)
            {
                return estimation;
            }

            room.CurrentEstimationId = estimationId;
            room.LastActivityAt = DateTimeOffset.UtcNow;

            await _estimationStore.SetCurrentAsync(room, cancellationToken);
            await _events.RoomUpdatedAsync(roomId, cancellationToken);

            return estimation;
        }

        private async Task<Room> GetRoomAsync(
            Guid roomId,
            CancellationToken cancellationToken)
        {
            return await _roomStore.GetAsync(roomId, cancellationToken)
                ?? throw new KeyNotFoundException("Room not found.");
        }

        private async Task<Estimation> GetEstimationAsync(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken)
        {
            return await _estimationStore.GetAsync(
                roomId, estimationId, cancellationToken)
                ?? throw new KeyNotFoundException("Estimation not found.");
        }

        private static void ValidateDetails(
            string title,
            string? description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);

            if (title.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Title must not exceed 200 characters.",
                    nameof(title));
            }

            if (description?.Length > 2000)
            {
                throw new ArgumentException(
                    "Description must not exceed 2000 characters.",
                    nameof(description));
            }
        }
    }
}