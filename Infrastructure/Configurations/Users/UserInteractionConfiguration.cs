
// Infrastructure/Persistence/Configurations/Users/UserInteractionConfiguration.cs

using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Users;

public class UserInteractionConfiguration : AuditEntityConfiguration<UserInteraction>
{
    public override void Configure(EntityTypeBuilder<UserInteraction> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserInteractions");

        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.MemberId)
            .IsRequired();

        builder.Property(ui => ui.BookId)
            .IsRequired();

        builder.Property(ui => ui.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ui => ui.InteractionDate)
            .IsRequired();

        // Propiedades opcionales según el tipo de interacción
        builder.Property(ui => ui.Rating)
            .IsRequired(false)
            .HasColumnType("int");

        builder.Property(ui => ui.Review)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(ui => ui.TimeSpentMinutes)
            .IsRequired(false)
            .HasDefaultValue(0);

        builder.Property(ui => ui.SearchQuery)
            .HasMaxLength(500)
            .IsRequired(false);

        // Índices
        builder.HasIndex(ui => ui.MemberId)
            .HasDatabaseName("IX_UserInteractions_MemberId");

        builder.HasIndex(ui => ui.BookId)
            .HasDatabaseName("IX_UserInteractions_BookId");

        builder.HasIndex(ui => ui.Type)
            .HasDatabaseName("IX_UserInteractions_Type");

        builder.HasIndex(ui => ui.InteractionDate)
            .HasDatabaseName("IX_UserInteractions_InteractionDate");

        // Índice compuesto para consultas de interacciones por miembro y tipo
        builder.HasIndex(ui => new { ui.MemberId, ui.Type })
            .HasDatabaseName("IX_UserInteractions_MemberId_Type");

        // Índice compuesto para análisis de libros populares
        builder.HasIndex(ui => new { ui.BookId, ui.Type, ui.InteractionDate })
            .HasDatabaseName("IX_UserInteractions_BookId_Type_Date");

        // Índice para ratings (solo aplica cuando Rating no es null)
        builder.HasIndex(ui => ui.Rating)
            .HasDatabaseName("IX_UserInteractions_Rating")
            .HasFilter("[Rating] IS NOT NULL");

        // Relaciones
        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(ui => ui.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Core.Books.Book>()
            .WithMany()
            .HasForeignKey(ui => ui.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Validación con check constraint
        builder.HasCheckConstraint("CK_UserInteraction_Rating", 
            "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
    }
}