using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Book;

public class BookConfiguration : AuditEntityConfiguration<Core.Books.Book>
{

    public override void Configure(EntityTypeBuilder<Core.Books.Book> builder)
    {
        base.Configure(builder);

        builder.ToTable("Books");

        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Isbn)
            .IsRequired()
            .HasMaxLength(13);
        
        builder.Property(b => b.PublicationYear)
            .IsRequired();

        builder.Property(b => b.Publisher)
            .HasMaxLength(100);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.HasIndex(b => b.Isbn).IsUnique();

        builder.HasIndex(b => b.Isbn);
        
        builder.HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property<List<Guid>>("_genreIds")
            .HasColumnName("GenreIds")
            .HasConversion(
                v => string.Join(',', v), 
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries) 
                    .Select(Guid.Parse)
                    .ToList()
            )
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        
        builder.Property<List<Guid>>("_copyIds")
            .HasColumnName("CopyIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);


    }
}