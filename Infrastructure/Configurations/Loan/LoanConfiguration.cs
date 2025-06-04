using Core.Books;
using Core.Loans;
using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Loan;

public class LoanConfiguration : AuditEntityConfiguration<Core.Loans.Loan>
{
    public override void Configure(EntityTypeBuilder<Core.Loans.Loan> builder)
    {
        base.Configure(builder);

        builder.ToTable("Loans");

        builder.HasKey(x => x.Id);
        
        builder.Property(l => l.LoanDate)
            .IsRequired();

        builder.Property(l => l.DueDate)
            .IsRequired();

        builder.Property(l => l.ReturnDate);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(l => l.RenewalCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(l => l.MaxRenewals)
            .IsRequired()
            .HasDefaultValue(2);

        builder.Property(l => l.Notes)
            .HasMaxLength(1000);
        
        builder.HasIndex(l => l.MemberId);
        builder.HasIndex(l => l.BookCopyId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => new { l.DueDate, l.ReturnDate });

        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<BookCopy>()
            .WithMany()
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Fine>()
            .WithMany()
            .HasForeignKey(l => l.FineId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}