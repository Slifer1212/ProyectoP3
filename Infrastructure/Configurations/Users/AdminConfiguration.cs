using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Users;

public class AdminConfiguration : AuditEntityConfiguration<Admin>
{
    public override void Configure(EntityTypeBuilder<Admin> builder)
    {
        base.Configure(builder);

        builder.ToTable("Admins");

        builder.HasKey(a => a.Id);

        // Propiedades heredadas de LibraryUser
        builder.Property(a => a.IdentityUserId)
            .IsRequired();

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired(false); // Opcional según tu modelo

        // Propiedades heredadas de Person
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Propiedad Role
        builder.Property(a => a.Role)
            .HasMaxLength(50)
            .HasDefaultValue("Admin")
            .ValueGeneratedOnAdd();

        // Propiedades de permisos
        builder.Property(a => a.CanManageBooks)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CanManageUsers)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(a => a.Email)
            .IsUnique()
            .HasDatabaseName("IX_Admins_Email");

        builder.HasIndex(a => a.IdentityUserId)
            .IsUnique()
            .HasDatabaseName("IX_Admins_IdentityUserId");

        // Índice para búsquedas por nombre
        builder.HasIndex(a => new { a.Name, a.LastName })
            .HasDatabaseName("IX_Admins_Name_LastName");

        // Índice para filtrar por permisos
        builder.HasIndex(a => new { a.CanManageBooks, a.CanManageUsers })
            .HasDatabaseName("IX_Admins_Permissions");

        // Configuración de propiedades computadas
        builder.Ignore(a => a.FullName);
    }
}