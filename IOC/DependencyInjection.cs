using Application.Dtos.BookDto;
using Application.Dtos.GenreDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Services;
using Application.Validators.Bussiness.Implementations;
using Application.Validators.Bussiness.Interfaces;
using Application.Validators.Format.BookValidations;
using Application.Validators.Format.GenreValidations;
using FluentValidation;
using Infraestructure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories & Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Services
        services.AddScoped<IBookService, BookService>();
        
        services.AddScoped<IBussinessValidator<CreateBookDto , UpdateBookDto ,  Guid>, BookBusinessValidator>();
        
        
        services.AddScoped<IValidator<CreateBookDto>, CreateBookFormatValidator>();
        services.AddScoped<IValidator<UpdateBookDto>, UpdateBookFormValidator>();
        services.AddScoped<IValidator<CreateGenreDto>,CreateGenreValidatior>();
        services.AddScoped<IValidator<UpdateGenreDto>,UpdateGenreValidator>();

        // Mapster
        services.AddSingleton<IMapper, Mapper>();
        
        return services;
    }
}