using Core.Loans;
using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Loan;

public class ReservationConfiguration : AuditEntityConfiguration<Reservation>
{
    public override void Configure(EntityTypeBuilder<Reservation> builder)
    {
        base.Configure(builder);

        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.BookId)
            .IsRequired();

        builder.Property(r => r.MemberId)
            .IsRequired();

        builder.Property(r => r.ReservationDate)
            .IsRequired();

        builder.Property(r => r.ExpirationDate)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.QueuePosition)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(r => r.NotifiedDate)
            .IsRequired(false);

        builder.Property(r => r.ReadyDate)
            .IsRequired(false);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        // Índices
        builder.HasIndex(r => r.BookId)
            .HasDatabaseName("IX_Reservations_BookId");

        builder.HasIndex(r => r.MemberId)
            .HasDatabaseName("IX_Reservations_MemberId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reservations_Status");

        builder.HasIndex(r => r.ExpirationDate)
            .HasDatabaseName("IX_Reservations_ExpirationDate");

        // Índice compuesto para consultas de reservas activas por libro
        builder.HasIndex(r => new { r.BookId, r.Status, r.QueuePosition })
            .HasDatabaseName("IX_Reservations_BookId_Status_QueuePosition");

        // Índice compuesto para consultas de reservas por miembro y estado
        builder.HasIndex(r => new { r.MemberId, r.Status })
            .HasDatabaseName("IX_Reservations_MemberId_Status");

        // Relaciones
        builder.HasOne<Core.Books.Book>()
            .WithMany()
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}