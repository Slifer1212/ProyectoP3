using Core.Books;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Book;

public class GenreConfiguration : AuditEntityConfiguration<Genre>
{
    public override void Configure(EntityTypeBuilder<Genre> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Genres");
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(g => g.Description)
            .HasMaxLength(500);
        
        
    }
}