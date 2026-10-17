namespace PlanningPoker.Core.Entities
{
    public sealed class Estimation
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRevealed { get; set; }

        public float? FinalValue { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
