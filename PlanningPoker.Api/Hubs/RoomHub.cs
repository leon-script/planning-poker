using Microsoft.AspNetCore.SignalR;

namespace PlanningPoker.Api.Hubs
{
    public sealed class RoomHub : Hub
    {
        public static string GroupName(Guid roomId)
            => $"room:{roomId:N}";

        public Task JoinRoom(Guid roomId)
        {
            if (roomId == Guid.Empty)
                throw new HubException("Invalid room ID.");

            return Groups.AddToGroupAsync(
                Context.ConnectionId,
                GroupName(roomId));
        }

        public Task LeaveRoom(Guid roomId)
        {
            return Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GroupName(roomId));
        }
    }
}