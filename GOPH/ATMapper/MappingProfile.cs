using AutoMapper;
using GOPH.Dto;
using GOPH.Entites;

namespace GOPH.ATMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //CommodityGroup
            CreateMap<CommodityGroup, CommodityGroupDto>();
            CreateMap<Commodity, CommodityDto>();


            //Product
            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductCart>()
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price));
            CreateMap<Product, ProductDetailDto>();
            CreateMap<Product, ProductForUpdateDto>();
            CreateMap<ProductForCreateDto, Product>();
            CreateMap<Product, ProductForCreateDto>();
            CreateMap<Product, ProductForContentUpdate>();
            CreateMap<ProductForCreateDto, Product>()
                .ForMember(dest => dest.IsPrice, opt => opt.MapFrom(src => src.GetIsPice()))
                .ForMember(dest => dest.Hot, opt => opt.MapFrom(src => src.GetIsHot()));

            CreateMap<ProductForUpdateDto, Product>()
                .ForMember(dest => dest.IsPrice, opt => opt.MapFrom(src => src.GetIsPice()))
                .ForMember(dest => dest.Hot, opt => opt.MapFrom(src => src.GetIsHot()));


            //Customer
            CreateMap<CustomerCreateDto, Customer>();
            CreateMap<Customer, CustomerCreateDto>();

            //Voucher
            CreateMap<Voucher, VouCherBidingDto>();
            CreateMap<VouCherBidingDto, Voucher>();

            //Receiver
       
            //Order
            CreateMap<Order, OrderDetailDto>();


            //IssueAnInvoice
            CreateMap<Invoice, InvoiceDetailDto>();


            //Image
            CreateMap<GOPH.Entites.Image, ImageForUpdateDto>();

            //Event

            CreateMap<Event, EventCreateDto>();
            CreateMap<EventCreateDto, Event>();

            CreateMap<Event, EventUpdateDto>();
            CreateMap<EventUpdateDto, Event>();


        }
    }
}
