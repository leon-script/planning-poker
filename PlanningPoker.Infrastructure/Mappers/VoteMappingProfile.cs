using AutoMapper;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Mappers
{
    public sealed class VoteMappingProfile : Profile
    {
        public VoteMappingProfile()
        {
            CreateMap<VoteDao, Vote>()
                .ReverseMap();
        }
    }
}
