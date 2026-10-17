namespace PlanningPoker.Core.Entities
{
    public class Vote
    {
        public Guid Id { get; set; }

        public Guid EstimationId { get; set; }

        public Guid ParticipantId { get; set; }

        public float Value { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
