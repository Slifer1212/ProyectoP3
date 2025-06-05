using Application.Dtos.BookDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Core.Books;
using MapsterMapper;

namespace Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
        var books = _unitOfWork.Books.GetAllAsync();
        return await books.ContinueWith(task =>
        {
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(task.Result).ToList();
            return OperationResult<IEnumerable<BookDto>>.Success(bookDtos);
        });
    }

    public async Task<OperationResult<BookDto>> CreateAsync(CreateBookDto dto)
    {
        // Verificar si el ISBN ya existe
        var existingBook = await _unitOfWork.Books.GetByIsbnAsync(dto.Isbn);
        if (existingBook != null)
            return OperationResult<BookDto>.Failure("A book with this ISBN already exists");

        // Validar que el autor existe
        var author = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
        if (author == null)
            return OperationResult<BookDto>.Failure("Author not found");

        // Validar géneros
        var genres = new List<Genre>();
        foreach (var genreId in dto.GenreIds)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(genreId);
            if (genre == null)
                return OperationResult<BookDto>.Failure($"Genre with ID {genreId} not found");
            genres.Add(genre);
        }

        // Crear el libro usando el factory method del dominio
        var bookResult = Book.Create(
            dto.Title,
            dto.Isbn,
            dto.PublicationYear,
            dto.AuthorId,
            author,
            genres.First(),
            dto.Publisher,
            dto.Description
        );

        if (!bookResult.IsSuccess)
            return OperationResult<BookDto>.Failure(bookResult.ErrorMessage);

        var book = bookResult.Data;

        // Agregar los géneros adicionales
        foreach (var genre in genres.Skip(1))
        {
            var addGenreResult = book.AddGenre(genre.Id);
            if (!addGenreResult.IsSuccess)
                return OperationResult<BookDto>.Failure(addGenreResult.ErrorMessage);
        }

        // Guardar en el repositorio
        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        var bookDto = _mapper.Map<BookDto>(book);
        return OperationResult<BookDto>.Success(bookDto);
    }

    public Task<OperationResult<BookDto>> UpdateAsync(Guid id, UpdateBookDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
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