using AutoMapper;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Mappers
{
    public sealed class EstimationMappingProfile : Profile
    {
        public EstimationMappingProfile()
        {
            CreateMap<EstimationDao, Estimation>()
                .ReverseMap();
        }
    }
}
