using Application.Dtos.BookDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Validators.Bussiness.Interfaces;
using Core.Books;
using FluentValidation;
using MapsterMapper;

namespace Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateBookDto> _createValidator;
    private readonly IValidator<UpdateBookDto> _updateValidator;
    private readonly IBussinessValidator<CreateBookDto , UpdateBookDto ,Guid> _businessValidator;
    public BookService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateBookDto> createValidator, IValidator<UpdateBookDto> updateValidator, IBussinessValidator<CreateBookDto, UpdateBookDto, Guid> businessValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _businessValidator = businessValidator;
    }

    public async Task<OperationResult<BookDto>> GetByIdAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetBookWithCopiesAsync(id);
        if (book == null)
            return OperationResult<BookDto>.Failure("Book not found");

        var bookDto = _mapper.Map<BookDto>(book);
        return OperationResult<BookDto>.Success(bookDto);
    }

    public async Task<OperationResult<IEnumerable<BookDto>>> GetAllAsync()
    {
        var books = await _unitOfWork.Books.GetAllAsync();
        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
        return OperationResult<IEnumerable<BookDto>>.Success(bookDtos);
    }

    public async Task<OperationResult<BookDto>> CreateAsync(CreateBookDto dto)
    {
          // 1. Format validation (synchronous)
        var formatValidation = await _createValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<BookDto>.Failure(string.Join("; ", errors));
        }

        // 2. Business/DB validation (asynchronous)
        var (isValid, businessErrors) = await _businessValidator.ValidateCreateAsync(dto);
        if (!isValid)
        {
            return OperationResult<BookDto>.Failure(string.Join("; ", businessErrors));
        }

        // 3. Get required entities (already validated)
        var author = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
        var genres = new List<Genre>();
        foreach (var genreId in dto.GenreIds ?? new List<Guid>())
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(genreId);
            genres.Add(genre!); // We know it exists due to prior validation
        }

        // 4. Create the book using the domain factory method
        var bookResult = Book.Create(
            dto.Title,
            dto.Isbn,
            dto.PublicationYear,
            dto.AuthorId,
            author!,
            genres.First(),
            dto.Publisher,
            dto.Description
        );

        if (!bookResult.IsSuccess)
            return OperationResult<BookDto>.Failure(bookResult.ErrorMessage);

        var book = bookResult.Data;

        // 5. Add additional genres
        foreach (var genre in genres.Skip(1))
        {
            var addGenreResult = book.AddGenre(genre.Id);
            if (!addGenreResult.IsSuccess)
                return OperationResult<BookDto>.Failure(addGenreResult.ErrorMessage);
        }

        try
        {
            // 6. Save to repository
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            // 7. Reload the book with all its relationships
            var savedBook = await _unitOfWork.Books.GetBookWithCopiesAsync(book.Id);
            var bookDto = _mapper.Map<BookDto>(savedBook);

            return OperationResult<BookDto>.Success(bookDto);
        }
        catch (Exception ex)
        {
            return OperationResult<BookDto>.Failure($"Error saving book: {ex.Message}");
        }
    }

    public async Task<OperationResult<BookDto>> UpdateAsync(Guid id, UpdateBookDto dto)
    {
        // Ensure the ID matches
        dto.Id = id;

        // 1. Format validation
        var formatValidation = await _updateValidator.ValidateAsync(dto);
        if (!formatValidation.IsValid)
        {
            var errors = formatValidation.Errors.Select(e => e.ErrorMessage).ToList();
            return OperationResult<BookDto>.Failure(string.Join("; ", errors));
        }

        // 2. Business/DB validation
        var (isValid, businessErrors) = await _businessValidator.ValidateUpdateAsync(dto);
        if (!isValid)
        {
            return OperationResult<BookDto>.Failure(string.Join("; ", businessErrors));
        }

        // 3. Get the existing book (already validated it exists)
        var book = await _unitOfWork.Books.GetByIdAsync(id);

        // 4. Update simple properties
        var titleResult = book!.UpdateTitle(dto.Title);
        if (!titleResult.IsSuccess)
            return OperationResult<BookDto>.Failure(titleResult.ErrorMessage);

        var descriptionResult = book.UpdateDescription(dto.Description);
        if (!descriptionResult.IsSuccess)
            return OperationResult<BookDto>.Failure(descriptionResult.ErrorMessage);

        // Update other fields directly
        book.Isbn = dto.Isbn;
        book.PublicationYear = dto.PublicationYear;
        book.Publisher = dto.Publisher;

        // 5. Update author if changed
        if (book.Author.Id != dto.AuthorId)
        {
            var newAuthor = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
            book.Author = newAuthor!; // We know it exists due to validation
        }

        // 6. Update genres
        // First remove all current genres
        var currentGenreIds = book.GenreIds.ToList();
        foreach (var genreId in currentGenreIds)
        {
            var removeResult = book.RemoveGenre(genreId);
            if (!removeResult.IsSuccess)
                return OperationResult<BookDto>.Failure(removeResult.ErrorMessage);
        }

        // Add the new genres
        foreach (var genreId in dto.GenreIds)
        {
            var addResult = book.AddGenre(genreId);
            if (!addResult.IsSuccess)
                return OperationResult<BookDto>.Failure(addResult.ErrorMessage);
        }

        try
        {
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            var updatedBook = await _unitOfWork.Books.GetBookWithCopiesAsync(book.Id);
            var bookDto = _mapper.Map<BookDto>(updatedBook);

            return OperationResult<BookDto>.Success(bookDto);
        }
        catch (Exception ex)
        {
            return OperationResult<BookDto>.Failure($"Error updating book: {ex.Message}");
        }
    }

    public async Task<OperationResult<bool>> DeleteAsync(Guid id)
    {
        // Business validation for deletion
        var (isValid, businessErrors) = await _businessValidator.ValidateDeleteAsync(id);
        if (!isValid)
        {
            return OperationResult<bool>.Failure(string.Join("; ", businessErrors));
        }

        var book = await _unitOfWork.Books.GetByIdAsync(id);

        // Soft delete using the domain method
        var deactivateResult = book!.Deactivate();
        if (!deactivateResult.IsSuccess)
            return OperationResult<bool>.Failure(deactivateResult.ErrorMessage);

        try
        {
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Failure($"Error deleting book: {ex.Message}");
        }    
    }

    public async Task<OperationResult<IEnumerable<BookDto>>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return OperationResult<IEnumerable<BookDto>>.Failure("Search term cannot be empty");

        var books = await _unitOfWork.Books.SearchBooksAsync(searchTerm);
        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

        return OperationResult<IEnumerable<BookDto>>.Success(bookDtos);
    }
}