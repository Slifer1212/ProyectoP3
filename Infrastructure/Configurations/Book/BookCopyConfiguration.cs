using Core.Books;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Book;

public class BookCopyConfiguration : AuditEntityConfiguration<BookCopy>
{
    public override void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        base.Configure(builder);

        builder.ToTable("BookCopies");

        builder.HasKey(bc => bc.Id);
        
        builder.Property(bc => bc.Barcode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(bc => bc.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(bc => bc.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bc => bc.Condition)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(bc => bc.Notes)
            .HasMaxLength(500);

        builder.Property(bc => bc.LastInventoryDate);
        
        builder.HasIndex(bc => bc.Barcode)
            .IsUnique();

        builder.HasIndex(bc => bc.Status);

        builder.HasIndex(bc => new { bc.BookId, bc.Status });
        
        builder.HasOne<Core.Books.Book>()
            .WithMany()
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}