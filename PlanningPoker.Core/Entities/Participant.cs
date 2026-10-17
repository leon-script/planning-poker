namespace PlanningPoker.Core.Entities
{
    public sealed class Participant
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset JoinedAt { get; set; }
    }
}
