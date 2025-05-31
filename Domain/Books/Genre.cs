using Core.BaseEntities;

namespace Core.Books;

public class Genre : AuditEntity
{
    public Guid Id {get; private set;}
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    
    private readonly List<Guid> _bookIds = new();
    public IReadOnlyList<Guid> BookIds => _bookIds.AsReadOnly();
    
    private Genre() { }

    public static DomainResult<Genre> Create(string name, string description)
    {
        var errors  = new List<string>();
        
        if(string.IsNullOrEmpty(description))
            errors.Add("Description is required");
        
        if (string.IsNullOrEmpty(name))
            errors.Add("Name is required");
        
        if (errors.Any())
            return DomainResult<Genre>.Failure(errors);
        
        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
        
        return DomainResult<Genre>.Success(genre);
    }

    public DomainResult UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult.Failure("Name cannot be empty.");
        
        Name = name;
        return DomainResult.Success();
    }

    public DomainResult UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return DomainResult.Failure("Description cannot be empty.");
        
        Description = description;
        return DomainResult.Success();
    }

    public DomainResult Activate()
    {
        if (IsDeleted == true)
            return DomainResult.Success();
        
        IsDeleted = false;
        return DomainResult.Success();
    }

    public DomainResult Deactivate()
    {
        if (IsDeleted == false)
            return DomainResult.Success();
        
        IsDeleted = true ;
        return DomainResult.Success();
    }
}