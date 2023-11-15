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

            CreateMap<Product, ProductDto>();

            CreateMap<Commodity, CommodityDto>();

            CreateMap<Product, ProductForCreateDto>();

            CreateMap<ProductForCreateDto, Product>()
                .ForMember(dest => dest.IsPrice, opt => opt.MapFrom(src => src.GetIsPice()))
                .ForMember(dest => dest.Hot, opt => opt.MapFrom(src => src.GetIsHot()));

            CreateMap<ProductForUpdateDto, Product>()
                .ForMember(dest => dest.IsPrice, opt => opt.MapFrom(src => src.GetIsPice()))
                .ForMember(dest => dest.Hot, opt => opt.MapFrom(src => src.GetIsHot()));

            CreateMap<Product, ProductForCreateDto>();

            CreateMap<Product, ProductCart>()
                 .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price));

        }
    }
}
