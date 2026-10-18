using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Infrastructure.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Stores;

namespace PlanningPoker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("PlanningPoker")
                ?? throw new InvalidOperationException("Connection string 'PlanningPoker' not found.");

            services.AddDbContext<PlanningPokerDbContext>(
                options => options.UseSqlServer(connectionString));

            // Stores
            services.AddScoped<IRoomStore, RoomStore>();
            services.AddScoped<IParticipantStore, ParticipantStore>();
            services.AddScoped<IEstimationStore, EstimationStore>();
            services.AddScoped<IVoteStore, VoteStore>();

            // AutoMapper
            services.AddAutoMapper(
                _ => { },
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
