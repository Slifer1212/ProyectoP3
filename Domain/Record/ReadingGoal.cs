using Core.BaseEntities;

namespace Core.Record;

public record ReadingGoal
{
    public int BooksPerYear { get; }
    public int Year { get; }
    public int CurrentProgress { get; private set; }
    public DateTime StartDate { get; }
    
    public ReadingGoal(int booksPerYear, int year)
    {
        BooksPerYear = booksPerYear;
        Year = year;
        CurrentProgress = 0;
        StartDate = new DateTime(year, 1, 1);
    }

    public static DomainResult<ReadingGoal> Create(int booksPerYear, int year)
    {
        var errors = new List<string>();
        if (booksPerYear <= 0)
            errors.Add("Books per year must be greater than zero");
        
        if (year < DateTime.Now.Year - 1)
            errors.Add("Year must be the current year or later");
        
        if(errors.Any())
            return DomainResult<ReadingGoal>.Failure(errors);

        var readingGoal = new ReadingGoal(booksPerYear, year);
        return DomainResult<ReadingGoal>.Success(readingGoal);
    }

    public decimal ProgressPercentage => CurrentProgress / (decimal)BooksPerYear * 100;
    public bool IsCompleted => CurrentProgress >= BooksPerYear;
    public int RemainingBooks => Math.Max(0, BooksPerYear - CurrentProgress);
    
    public DomainResult UpdateProgress(int newProgress)
    {
        if (newProgress < 0)
            return DomainResult.Failure("Progress cannot be negative");
            
        CurrentProgress = newProgress;
        return DomainResult.Success();
    }
}