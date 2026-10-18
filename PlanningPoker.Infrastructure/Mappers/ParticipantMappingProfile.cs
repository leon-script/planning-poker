using AutoMapper;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Mappers
{
    public sealed class ParticipantMappingProfile : Profile
    {
        public ParticipantMappingProfile()
        {
            CreateMap<ParticipantDao, Participant>()
                .ReverseMap();
        }
    }
}
