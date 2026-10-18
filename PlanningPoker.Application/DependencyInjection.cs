using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Services;

namespace PlanningPoker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Application services
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IParticipantService, ParticipantService>();
            services.AddScoped<IEstimationService, EstimationService>();
            services.AddScoped<IVoteService, VoteService>();

            // AutoMapper
            services.AddAutoMapper(_ => { }, typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
