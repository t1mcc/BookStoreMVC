using AutoMapper;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Models.ViewModels;

namespace BookStore
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<RegisterViewModel, User>()
                     //
                     .ForMember(dest => dest.PhoneNumberConfirmed,
                                opt => opt.MapFrom(src => true))
                     .ForMember(dest => dest.EmailConfirmed,
                                opt => opt.MapFrom(src => true));

            CreateMap<BookViewModel, Book>()
                     .ForMember(dest => dest.Photo,
                                opt => opt.MapFrom(src => src.Photo.FileName))
                     .ReverseMap()
                     .ForMember(dest => dest.Photo, opt => opt.Ignore());

            CreateMap<User, UserProfileViewModel>()
                    .ForMember(dest => dest.Role, 
                               opt => opt.MapFrom<RoleResolver>())
                    .ReverseMap();
        }
    }
}
