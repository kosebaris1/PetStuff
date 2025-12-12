using AutoMapper;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //  → Entity
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>()
               .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<RemoveProductCommand, Product>().ReverseMap();
        }
    }
}
