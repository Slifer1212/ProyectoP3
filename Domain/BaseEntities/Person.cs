namespace Core.BaseEntities;

public abstract class Person : AuditEntity
{
    public string? Name { get; set; }
    public string? LastName { get; set; }

    public string? FullName => $"{Name} {LastName}";
    
    protected Person(){}
}