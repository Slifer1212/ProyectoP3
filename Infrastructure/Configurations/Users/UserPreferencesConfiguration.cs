using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Configurations.Users;

public class UserPreferencesConfiguration : AuditEntityConfiguration<UserPreferences>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserPreferences> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserPreferences");

        builder.HasKey(up => up.Id);
        
        
        builder.Property(up => up.MemberId)
            .IsRequired();
        
        // Configuración de notificaciones
        builder.Property(up => up.EmailNotifications)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.SmsNotifications)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(up => up.PushNotifications)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.LoanReminders)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.RecommendationAlerts)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.ReservationNotifications)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Preferencias de lectura
        builder.Property(up => up.PreferredLanguage)
            .HasMaxLength(50)
            .HasDefaultValue("English");
        
        
        // Configurar ReadingGoal como tipo complejo (owned type)
        builder.OwnsOne(up => up.CurrentReadingGoal, rg =>
        {
            rg.Property(r => r.BooksPerYear)
                .HasColumnName("ReadingGoal_BooksPerYear");
            
            rg.Property(r => r.Year)
                .HasColumnName("ReadingGoal_Year");
            
            rg.Property(r => r.CurrentProgress)
                .HasColumnName("ReadingGoal_CurrentProgress");
            
            rg.Property(r => r.StartDate)
                .HasColumnName("ReadingGoal_StartDate");
            
            rg.Ignore(r => r.ProgressPercentage);
            rg.Ignore(r => r.IsCompleted);
            rg.Ignore(r => r.RemainingBooks);
        });
        
        // Configurar listas privadas
        builder.Property<List<Guid>>("_favoriteGenreIds")
            .HasColumnName("FavoriteGenreIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<List<Guid>>("_favoriteAuthorIds")
            .HasColumnName("FavoriteAuthorIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Índices
        builder.HasIndex(up => up.MemberId)
            .IsUnique()
            .HasDatabaseName("IX_UserPreferences_MemberId");
        
        builder.HasOne<Member>()
            .WithOne()
            .HasForeignKey<UserPreferences>(up => up.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}