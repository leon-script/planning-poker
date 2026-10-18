using AutoMapper;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.Persistence.Daos;

namespace PlanningPoker.Infrastructure.Mappers
{
    public sealed class RoomMappingProfile : Profile
    {
        public RoomMappingProfile()
        {
            CreateMap<RoomDao, Room>()
                .ReverseMap();
        }
    }
}
