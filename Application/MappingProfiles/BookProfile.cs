using Application.DTOs.Books;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.BookBorrows, opt => opt.Ignore());
        }
    }
}
