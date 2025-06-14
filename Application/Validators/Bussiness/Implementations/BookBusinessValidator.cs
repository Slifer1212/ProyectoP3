using Application.Dtos.BookDto;
using Application.Interfaces;
using Application.Validators.Bussiness.Interfaces;

namespace Application.Validators.Bussiness.Implementations;

public class BookBusinessValidator : IBussinessValidator<CreateBookDto, UpdateBookDto, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public BookBusinessValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateCreateAsync(CreateBookDto dto)
    {
        var errors = new List<string>();

        // Check if ISBN already exists
        var existingBook = await _unitOfWork.Books.GetByIsbnAsync(dto.Isbn);
        if (existingBook != null)
        {
            errors.Add("A book with this ISBN already exists.");
        }

        // Validate author existence
        var author = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
        if (author == null)
        {
            errors.Add("Author not found.");
        }

        // Validate genres
        if (dto.GenreIds != null && dto.GenreIds.Any())
        {
            foreach (var genreId in dto.GenreIds)
            {
                var genre = await _unitOfWork.Genres.GetByIdAsync(genreId);
                if (genre == null)
                {
                    errors.Add($"Genre with ID {genreId} does not exist.");
                }
            }
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateUpdateAsync(UpdateBookDto dto)
    {
        var errors = new List<string>();

        // Check if book exists
        var existingBook = await _unitOfWork.Books.GetByIdAsync(dto.Id);
        if (existingBook == null)
        {
            errors.Add("Book not found.");
            return (false, errors);
        }

        // Check if ISBN already exists for another book
        if (dto.Isbn != existingBook.Isbn)
        {
            var bookWithSameIsbn = await _unitOfWork.Books.GetByIsbnAsync(dto.Isbn);
            if (bookWithSameIsbn != null)
            {
                errors.Add("A book with this ISBN already exists.");
            }
        }

        // Validate author existence
        var author = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
        if (author == null)
        {
            errors.Add("Author not found.");
        }

        // Validate genres
        if (dto.GenreIds != null && dto.GenreIds.Any())
        {
            foreach (var genreId in dto.GenreIds)
            {
                var genre = await _unitOfWork.Genres.GetByIdAsync(genreId);
                if (genre == null)
                {
                    errors.Add($"Genre with ID {genreId} does not exist.");
                }
            }
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateDeleteAsync(Guid bookId)
    {
        var errors = new List<string>();

        // Verify if the book exists
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book == null)
        {
            errors.Add("The specified book does not exist.");
            return (false, errors);
        }
        
        // Verify if the book has copies
        // TODO : Implement ICopyRepository
        
        /*
        var copies = await _unitOfWork.Copies.GetByBookIdAsync(bookId);
        if (copies == null || !copies.Any())
        {
            errors.Add("The book has no copies and cannot be deleted.");
            return (false, errors);
        }*/
        

        // TODO: Verificar si el libro tiene préstamos activos
        // cuando implementes ILoanRepository
        // var activeLoans = await _unitOfWork.Loans.GetActiveLoansByBookAsync(bookId);
        // if (activeLoans.Any())
        // {
        //     errors.Add("This book cannot be deleted because it has active loans.");
        // }

        return (errors.Count == 0, errors);
    }
}