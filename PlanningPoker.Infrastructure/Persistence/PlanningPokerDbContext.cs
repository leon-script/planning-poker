using Microsoft.EntityFrameworkCore;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.Persistence
{
    public sealed class PlanningPokerDbContext : DbContext
    {
        public PlanningPokerDbContext(
            DbContextOptions<PlanningPokerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Estimation> Estimations => Set<Estimation>();
        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Room
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.AccessCodeHash)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.HasIndex(x => x.AccessCodeHash)
                    .IsUnique();

                entity.HasMany<Participant>()
                    .WithOne()
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany<Estimation>()
                    .WithOne()
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Participant
            modelBuilder.Entity<Participant>(entity =>
            {
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
            modelBuilder.Entity<Estimation>(entity =>
            {
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
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Value)
                    .HasColumnType("real")
                    .IsRequired();

                entity.HasOne<Estimation>()
                    .WithMany()
                    .HasForeignKey(x => x.EstimationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Participant>()
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
