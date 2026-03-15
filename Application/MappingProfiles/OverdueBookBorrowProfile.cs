using Application.DTOs.Books;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class OverdueBookBorrowProfile: Profile
    {
        public OverdueBookBorrowProfile()
        {
            CreateMap<OverdueBorrow, OverdueBorrowDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}
