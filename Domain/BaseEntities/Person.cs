namespace Core.BaseEntities;

public abstract class Person : AuditEntity
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
}