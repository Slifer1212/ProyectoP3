namespace Core.Record;

public record ReadingGoal
{
    public int BooksPerYear { get; }
    public int Year { get; }
    public int CurrentProgress { get; }
    public DateTime StartDate { get; }
    
    public ReadingGoal(int booksPerYear, int year)
    {
        if (booksPerYear <= 0) throw new ArgumentException("Books per year must be positive");
        if (year < DateTime.Now.Year - 1) throw new ArgumentException("Invalid year");
        
        BooksPerYear = booksPerYear;
        Year = year;
        CurrentProgress = 0;
        StartDate = new DateTime(year, 1, 1);
    }
    
    public decimal ProgressPercentage => CurrentProgress / (decimal)BooksPerYear * 100;
    public bool IsCompleted => CurrentProgress >= BooksPerYear;
    public int RemainingBooks => Math.Max(0, BooksPerYear - CurrentProgress);
}