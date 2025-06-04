using Core.Users;
using Infraestructure.Configurations.BaseConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.Users;

public class LibrarianConfiguration : AuditEntityConfiguration<Librarian>
{
    public override void Configure(EntityTypeBuilder<Librarian> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Librarian");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.IdentityUserId)
            .IsRequired();
        
        builder.Property(l => l.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(l => l.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(l => l.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.HireDate)
            .IsRequired();

        builder.Property(l => l.CanManageBooks)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(l => l.CanManageUsers)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(l => l.Role)
            .HasMaxLength(50)
            .HasDefaultValue("Librarian")
            .ValueGeneratedOnAdd();

        // Índices
        builder.HasIndex(l => l.Email)
            .IsUnique()
            .HasDatabaseName("IX_Librarians_Email");

        builder.HasIndex(l => l.IdentityUserId)
            .IsUnique()
            .HasDatabaseName("IX_Librarians_IdentityUserId");

        builder.HasIndex(l => l.Department)
            .HasDatabaseName("IX_Librarians_Department");

        builder.HasIndex(l => new { l.Department, l.HireDate })
            .HasDatabaseName("IX_Librarians_Department_HireDate");

        // Índice para búsquedas por nombre
        builder.HasIndex(l => new { l.Name, l.LastName })
            .HasDatabaseName("IX_Librarians_Name_LastName");

        // Índice para filtrar por permisos
        builder.HasIndex(l => new { l.CanManageBooks, l.CanManageUsers })
            .HasDatabaseName("IX_Librarians_Permissions");

        // Configuración de propiedades computadas
        builder.Ignore(l => l.FullName);
        
    }
}