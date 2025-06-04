using Application.Dtos.AuthorDto;
using Application.Dtos.BookDto;
using Application.Dtos.GenreDto;
using AutoMapper;
using Core.Books;

namespace Application.Mappings;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AvailableCopies, 
                opt => opt.MapFrom(src => src.CopyIds.Count)); 

        CreateMap<Author, AuthorDto>();
        CreateMap<Genre, GenreDto>();
    }
}