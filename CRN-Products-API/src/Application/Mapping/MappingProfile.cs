using AutoMapper;
using CRN.Application.DTOs;
using CRN.Domain.Entities;

namespace CRN.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}
