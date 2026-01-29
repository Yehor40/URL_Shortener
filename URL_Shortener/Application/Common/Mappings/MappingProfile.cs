using AutoMapper;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Domain.Entities;

namespace URL_Shortener.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShortUrl, ShortUrlDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.UserName : "Anonymous"));
    }
}
