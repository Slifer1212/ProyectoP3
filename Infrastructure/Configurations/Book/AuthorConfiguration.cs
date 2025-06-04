using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Book;

public class AuthorConfiguration : AuditEntityConfiguration<Core.Books.Author>
{
    public override void Configure(EntityTypeBuilder<Core.Books.Author> builder)
    {
        base.Configure(builder);    
        
        builder.ToTable("Authors");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("Name");
        
        builder.Property(a => a.LastName).
            IsRequired()
            .HasMaxLength(50)
            .HasColumnName("LastName");

        builder.Property(a => a.Biography)
            .HasMaxLength(2000);
        
        builder.Property(a => a.DeathDate)
            .IsRequired(false);

        builder.Property(a => a.Nationality)
            .IsRequired()
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasIndex(a => new { a.Name, a.LastName });
        
        builder.Property<List<Guid>>("_bookIds")
            .HasColumnName("BookIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
        
    }
}