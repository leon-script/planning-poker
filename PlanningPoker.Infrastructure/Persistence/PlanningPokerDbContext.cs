using Microsoft.EntityFrameworkCore;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Persistence
{
    public sealed class PlanningPokerDbContext : DbContext
    {
        public PlanningPokerDbContext(DbContextOptions<PlanningPokerDbContext> options) : base(options)
        {
        }

        public DbSet<RoomDao> Rooms => Set<RoomDao>();
        public DbSet<ParticipantDao> Participants => Set<ParticipantDao>();
        public DbSet<EstimationDao> Estimations => Set<EstimationDao>();
        public DbSet<VoteDao> Votes => Set<VoteDao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Room
            modelBuilder.Entity<RoomDao>(entity =>
            {
                entity.ToTable("Rooms");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AccessCodeHash)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.HasIndex(x => x.AccessCodeHash)
                    .IsUnique();

                entity.HasMany<ParticipantDao>()
                    .WithOne()
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany<EstimationDao>()
                    .WithOne()
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Participant
            modelBuilder.Entity<ParticipantDao>(entity =>
            {
                entity.ToTable("Participants");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.TokenHash)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.HasIndex(x => new
                {
                    x.RoomId,
                    x.Name,
                    x.Role
                }).IsUnique();

                entity.HasIndex(x => new
                {
                    x.RoomId,
                    x.TokenHash
                }).IsUnique();
            });

            // Estimation
            modelBuilder.Entity<EstimationDao>(entity =>
            {
                entity.ToTable("Estimations");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(2000);

                entity.Property(x => x.IsRevealed)
                    .IsRequired();

                entity.Property(x => x.FinalValue)
                    .HasColumnType("real");

                entity.HasIndex(x => x.RoomId);
            });

            // Vote
            modelBuilder.Entity<VoteDao>(entity =>
            {
                entity.ToTable("Votes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Value)
                    .HasColumnType("real")
                    .IsRequired();

                entity.HasOne<EstimationDao>()
                    .WithMany()
                    .HasForeignKey(x => x.EstimationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ParticipantDao>()
                    .WithMany()
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(x => new
                {
                    x.EstimationId,
                    x.ParticipantId
                }).IsUnique();
            });
        }
    }
}
