using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Configurations.Notifications;

public class NotificationsConfiguration : AuditEntityConfiguration<Core.Notifications.Notification>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Notifications.Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);
        
        builder.HasIndex(n => n.UserId);
        
        
    }
}