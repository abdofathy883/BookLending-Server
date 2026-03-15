using Application.DTOs.Books;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class BookBorrowProfile : Profile
    {
        public BookBorrowProfile()
        {
            CreateMap<BookBorrow, BookBorrowDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));
        }
    }
}
