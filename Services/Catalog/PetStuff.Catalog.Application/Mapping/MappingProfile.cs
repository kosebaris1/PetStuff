using AutoMapper;
using PetStuff.Catalog.Application.DTOs;
using PetStuff.Catalog.Application.Features.Brands.Commands;
using PetStuff.Catalog.Application.Features.Categories.Commands;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Commands → Entity
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>()
               .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<RemoveProductCommand, Product>().ReverseMap();

            // Entity → DTOs
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string>()));

            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => 
                    src.Images != null && src.Images.Any() 
                        ? (src.Images.FirstOrDefault(i => i.IsMain) != null 
                            ? src.Images.Where(i => i.IsMain).First().ImageUrl 
                            : src.Images.First().ImageUrl)
                        : (string?)null));

            CreateMap<Category, CategoryDto>();
            CreateMap<Brand, BrandDto>();

            // Category Commands
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>();

            // Brand Commands
            CreateMap<CreateBrandCommand, Brand>();
            CreateMap<UpdateBrandCommand, Brand>();
        }
    }
}
