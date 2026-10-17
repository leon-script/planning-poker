using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Hubs;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Api.Realtime
{
    public sealed class SignalRRoomEventPublisher : IRoomEventPublisher
    {
        private readonly IHubContext<RoomHub> _hubContext;

        public SignalRRoomEventPublisher(
            IHubContext<RoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task RoomUpdatedAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients
                .Group(RoomHub.GroupName(roomId))
                .SendAsync(
                    "RoomUpdated",
                    roomId,
                    cancellationToken);
        }

        public Task RoomDeletedAsync(
            Guid roomId,
            CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients
                .Group(RoomHub.GroupName(roomId))
                .SendAsync(
                    "RoomDeleted",
                    roomId,
                    cancellationToken);
        }
    }
}
