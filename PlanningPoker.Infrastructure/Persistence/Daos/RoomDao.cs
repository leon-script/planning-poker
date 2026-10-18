namespace PlanningPoker.Infrastructure.Persistence.Daos
{
    public class RoomDao
    {
        public Guid Id { get; set; }

        public string AccessCodeHash { get; set; } = string.Empty;

        public Guid? CurrentEstimationId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset LastActivityAt { get; set; }
    }
}
