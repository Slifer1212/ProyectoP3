using Application.Dtos.AuthorDto;
using Application.Dtos.BookDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Validators.Bussiness.Interfaces;
using Core.Books;
using FluentValidation;
using MapsterMapper;

namespace Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateAuthorDto> _createValidator;
    private readonly IValidator<UpdateAuthorDto> _updateValidator;
    private readonly IBussinessValidator<CreateAuthorDto, UpdateAuthorDto , Guid> _businessValidator;

    public AuthorService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateAuthorDto> createValidator,
        IValidator<UpdateAuthorDto> updateValidator, IBussinessValidator<CreateAuthorDto, UpdateAuthorDto, Guid> businessValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _businessValidator = businessValidator;
    }

    public async Task<OperationResult<AuthorDto>> GetByIdAsync(Guid id)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id);
        if (author == null)
            return OperationResult<AuthorDto>.Failure("Author not found");

        var authorDto = _mapper.Map<AuthorDto>(author);
        return OperationResult<AuthorDto>.Success(authorDto);
    }

    public async Task<OperationResult<IEnumerable<AuthorDto>>> GetAllAsync()
    {
        var authors = await _unitOfWork.Authors.GetAllAsync();
        var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
        return OperationResult<IEnumerable<AuthorDto>>.Success(authorDtos);
    }

    public async Task<OperationResult<AuthorDto>> CreateAsync(CreateAuthorDto dto)
    {
        // 1. Format validation
        var formatValidation = await _createValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<AuthorDto>.Failure(string.Join("; ", errors));
        }

        // 2. Business validation
        var (isValid, businessErrors) = await _businessValidator.ValidateCreateAsync(dto);
        if (!isValid)
        {
            return OperationResult<AuthorDto>.Failure(string.Join("; ", businessErrors));
        }

        // 3. Create the author using the domain factory method
        var authorResult = Author.Create(
            dto.Name ?? "",
            dto.LastName ?? "",
            dto.BirthDate ?? DateTime.MinValue,
            dto.DeathDate ?? DateTime.MaxValue,
            dto.Nationality ?? "",
            dto.Biography ?? ""
        );

        if (!authorResult.IsSuccess)
            return OperationResult<AuthorDto>.Failure(authorResult.ErrorMessage);

        var author = authorResult.Data;

        try
        {
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.SaveChangesAsync();

            var authorDto = _mapper.Map<AuthorDto>(author);
            return OperationResult<AuthorDto>.Success(authorDto);
        }
        catch (Exception ex)
        {
            return OperationResult<AuthorDto>.Failure($"Error saving author: {ex.Message}");
        }
    }

    public async Task<OperationResult<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto)
    {
        // Ensure the ID matches
        dto.Id = id;

        // 1. Format validation
        var formatValidation = await _updateValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<AuthorDto>.Failure(string.Join("; ", errors));
        }

        // 2. Business validation
        var (isValid, businessErrors) = await _businessValidator.ValidateUpdateAsync(dto);
        if (!isValid)
        {
            return OperationResult<AuthorDto>.Failure(string.Join("; ", businessErrors));
        }

        // 3. Get the existing author
        var author = await _unitOfWork.Authors.GetByIdAsync(id);

        // 4. Update properties
        author!.Name = dto.Name;
        author.LastName = dto.LastName;

        if (!string.IsNullOrWhiteSpace(dto.Biography))
        {
            var bioResult = author.UpdateBiography(dto.Biography);
            if (!bioResult.IsSuccess)
                return OperationResult<AuthorDto>.Failure(bioResult.ErrorMessage);
        }

        if (dto.BirthDate.HasValue)
        {
            var birthResult = author.SetBirthDate(dto.BirthDate.Value);
            if (!birthResult.IsSuccess)
                return OperationResult<AuthorDto>.Failure(birthResult.ErrorMessage);
        }

        // Update other fields directly
        author.DeathDate = dto.DeathDate;
        author.Nationality = dto.Nationality ?? author.Nationality;

        try
        {
            _unitOfWork.Authors.Update(author);
            await _unitOfWork.SaveChangesAsync();

            var authorDto = _mapper.Map<AuthorDto>(author);
            return OperationResult<AuthorDto>.Success(authorDto);
        }
        catch (Exception ex)
        {
            return OperationResult<AuthorDto>.Failure($"Error updating author: {ex.Message}");
        }
    }

    public async Task<OperationResult<bool>> DeleteAsync(Guid id)
    {
        // Business validation
        var (isValid, businessErrors) = await _businessValidator.ValidateDeleteAsync(id);
        if (!isValid)
        {
            return OperationResult<bool>.Failure(string.Join("; ", businessErrors));
        }

        var author = await _unitOfWork.Authors.GetByIdAsync(id);

        // Soft delete
        author!.IsDeleted = true;
        author.DeletedAt = DateTime.UtcNow;

        try
        {
            _unitOfWork.Authors.Update(author);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Failure($"Error deleting author: {ex.Message}");
        }
    }

    public async Task<OperationResult<IEnumerable<BookDto>>> GetBooksByAuthorAsync(Guid authorId)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(authorId);
        if (author == null)
            return OperationResult<IEnumerable<BookDto>>.Failure("Author not found");

        var books = await _unitOfWork.Books.GetByAuthorAsync(authorId);
        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

        return OperationResult<IEnumerable<BookDto>>.Success(bookDtos);
    }
}