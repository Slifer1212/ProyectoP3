using Application.Dtos.AuthorDto;
using Application.Dtos.BookDto;
using Application.Dtos.GenreDto;
using Core.Books;
using Mapster;

namespace Application.Mappings;

public static class MapsterConfig
{
    public static void ConfigureMappings()
    {
        // Book mappings
        TypeAdapterConfig<Book, BookDto>
            .NewConfig()
            .Map(dest => dest.Author, src => src.Author.Adapt<AuthorDto>())
            .Map(dest => dest.TotalCopies, src => src.TotalCopies);

        TypeAdapterConfig<CreateBookDto, Book>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Author)
            .Ignore(dest => dest.Genre)
            .ConstructUsing(src => Book.Create(
                src.Title,
                src.Isbn,
                src.PublicationYear,
                src.AuthorId,
                null!, // Se asignará en el servicio
                null!, // Se asignará en el servicio
                src.Publisher,
                src.Description
            ).Data);

        // Author mappings
        TypeAdapterConfig<Author, AuthorDto>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.LastName, src => src.LastName);

        TypeAdapterConfig<CreateAuthorDto, Author>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .ConstructUsing(src => Author.Create(
                src.Name ?? "",
                src.LastName ?? "",
                src.BirthDate ?? DateTime.MinValue,
                src.DeathDate ?? DateTime.MaxValue,
                src.Nationality ?? "",
                src.Biography ?? ""
            ).Data);

        // Genre mappings
        TypeAdapterConfig<Genre, GenreDto>
            .NewConfig();

        TypeAdapterConfig<CreateGenreDto, Genre>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .ConstructUsing(src => Genre.Create(
                src.Name ?? "",
                src.Description ?? ""
            ).Data);
    }
}