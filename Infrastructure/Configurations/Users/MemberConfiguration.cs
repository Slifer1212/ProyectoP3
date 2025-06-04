using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Users;

public class MemberConfiguration : AuditEntityConfiguration<Member>
{
    public override void Configure(EntityTypeBuilder<Member> builder)
    {
        base.Configure(builder);

        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.IdentityUserId)
            .IsRequired();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(m => m.MembershipExpiry)
            .IsRequired();

        builder.Property(m => m.MemberShipType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.MembershipStartDate)
            .IsRequired();

        builder.Property(m => m.OutstandingFines)
            .HasPrecision(10, 2);

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.MemberShipState)
            .IsRequired()
            .HasConversion<string>();

        // Índices
        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.HasIndex(m => m.IdentityUserId)
            .IsUnique();

        builder.HasIndex(m => m.MemberShipState);

        // Configurar listas privadas
        builder.Property<List<Guid>>("_activeLoanIds")
            .HasColumnName("ActiveLoanIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<List<Guid>>("_activeReservationIds")
            .HasColumnName("ActiveReservationIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<List<Guid>>("_fineIds")
            .HasColumnName("FineIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}