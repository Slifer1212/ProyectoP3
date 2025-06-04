using Core.Loans;
using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Loan;

public class FineConfiguration : AuditEntityConfiguration<Fine>{

    public override void Configure(EntityTypeBuilder<Fine> builder)
    {
        base.Configure(builder);

        builder.ToTable("Fines");
        
        builder.Property(f => f.Amount)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(f => f.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(f => f.IsPaid)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.PaidAt);

        builder.Property(f => f.PaidAmount)
            .HasPrecision(10, 2);

        builder.Property(f => f.PaymentMethod)
            .HasConversion<string>();

        
        builder.HasIndex(f => f.MemberId);
        builder.HasIndex(f => f.IsPaid);
        builder.HasIndex(f => new { f.MemberId, f.IsPaid });

        // Relaciones
        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(f => f.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}