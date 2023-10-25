using AutoMapper;
using GOPH.Dto;
using GOPH.Entites;

namespace GOPH.ATMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CommodityGroup, CommodityGroupDto>();
        }
    }
}
