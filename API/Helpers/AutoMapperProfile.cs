using API.DTO;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, MemberDto>()
                .ForMember(user => user.PhotoUrl,
                obj => obj.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain)!.Url));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, User>();
            CreateMap<RegisterDto, User>();
            CreateMap<string, DateOnly>().ConvertUsing(str => DateOnly.Parse(str));
        }
    }
}
