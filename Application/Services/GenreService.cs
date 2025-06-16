using Application.Dtos.BookDto;
using Application.Dtos.GenreDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Validators.Bussiness.Interfaces;
using Core.Books;
using FluentValidation;
using MapsterMapper;

namespace Application.Services;

public class GenreService : IGenreService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateGenreDto> _createValidator;
    private readonly IValidator<UpdateGenreDto> _updateValidator;
    private readonly IBussinessValidator<CreateGenreDto , UpdateGenreDto , Guid> _businessValidator;

    public GenreService(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateGenreDto> createValidator, IValidator<UpdateGenreDto> updateValidator, IBussinessValidator<CreateGenreDto, UpdateGenreDto, Guid> businessValidator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _businessValidator = businessValidator;
    }

    public async Task<OperationResult<GenreDto>> GetByIdAsync(Guid id)
    {
        var genre = await _unitOfWork.Genres.GetByIdAsync(id);
        if (genre == null)
            return OperationResult<GenreDto>.Failure("Genre not found");

        var genreDto = _mapper.Map<GenreDto>(genre);
        return OperationResult<GenreDto>.Success(genreDto);
    }

    public async Task<OperationResult<IEnumerable<GenreDto>>> GetAllAsync()
    {
        var genres = _unitOfWork.Genres.GetAllAsync();
        if (genres == null)
            return (OperationResult<IEnumerable<GenreDto>>.Failure("No genres found"));

        var genreDtos = _mapper.Map<IEnumerable<GenreDto>>(genres);
        return (OperationResult<IEnumerable<GenreDto>>.Success(genreDtos));
    }

    public async Task<OperationResult<GenreDto>> CreateAsync(CreateGenreDto dto)
    {
        // 1. Format validation
        var formatValidation = await _createValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<GenreDto>.Failure(string.Join("; ", errors));
        }
        
        // 2. Business validation
        var businessValidation = await _businessValidator.ValidateCreateAsync(dto);
        if (!businessValidation.IsValid)
        {
            return OperationResult<GenreDto>.Failure(string.Join("; ", businessValidation.Errors));
        }
        
        // 3. Create genre
        var genreResult = Genre.Create(
            dto.Name!,
            dto.Description!);

        // Check if genre creation was successful
        if (!genreResult.IsSuccess)
        {
            return OperationResult<GenreDto>.Failure(genreResult.ErrorMessage);
        }
        // Map the created genre to a DTO
        var genre = genreResult.Data;
        
        // Add the genre to the database
        await _unitOfWork.Genres.AddAsync(genre);
        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();
        
        // Map the genre to a DTO for the response
        var genreDto = _mapper.Map<GenreDto>(genre);
        // Return the successful operation result with the genre DTO
        return OperationResult<GenreDto>.Success(genreDto);
    }

    public async Task<OperationResult<GenreDto>> UpdateAsync(Guid id, UpdateGenreDto dto)
    {
        dto.Id = id;

        var existGenre = _unitOfWork.Genres.GetByIdAsync(id);
        
        if (existGenre == null)
            return OperationResult<GenreDto>.Failure("Genre not found");
        
        // 1. Format validation
        var formatValidation = await _updateValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<GenreDto>.Failure(string.Join("; ", errors));
        }
        
        // 2. Business validation
        
        var businessValidation = await _businessValidator.ValidateUpdateAsync(dto);
        if (!businessValidation.IsValid)
        {
            return OperationResult<GenreDto>.Failure(string.Join("; ", businessValidation.Errors));
        }
        // 3. Update genre
        
        var genre =  await _unitOfWork.Genres.GetByIdAsync(id);
        
        genre!.Description = dto.Description;
        genre.Name = dto.Name;
        
        // Add the updated genre to the database
        
         _unitOfWork.Genres.Update(genre);
        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();
        
        // Map the updated genre to a DTO for the response
        var genreDto = _mapper.Map<GenreDto>(genre);
        // Return the successful operation result with the genre DTO
        return OperationResult<GenreDto>.Success(genreDto);
        
    }

    public async Task<OperationResult<bool>> DeleteAsync(Guid id)
    {
        var businessValidation = await _businessValidator.ValidateDeleteAsync(id);
        
        if (!businessValidation.IsValid)
        {
            return OperationResult<bool>.Failure(string.Join("; ", businessValidation.Errors));
        }
        
        var genre = await _unitOfWork.Genres.GetByIdAsync(id);
        
        var deactivateResult = genre!.Deactivate();
        if (!deactivateResult.IsSuccess)
            return OperationResult<bool>.Failure(deactivateResult.ErrorMessage);
        
        try
        {
            _unitOfWork.Genres.Update(genre);
            await _unitOfWork.SaveChangesAsync();
            
            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Failure($"Error deleting genre: {ex.Message}");
        }
    }

    public Task<OperationResult<IEnumerable<BookDto>>> GetBooksByGenreAsync(Guid genreId)
    {
        var books = _unitOfWork.Books.GetByGenreAsync(genreId);
        if (books == null)
            return Task.FromResult(OperationResult<IEnumerable<BookDto>>.Failure("No books found for this genre"));

        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
        return Task.FromResult(OperationResult<IEnumerable<BookDto>>.Success(bookDtos));
    }
}