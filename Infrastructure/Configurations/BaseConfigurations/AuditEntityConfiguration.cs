using Core.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Configurations.BaseConfigurations;

public abstract class AuditEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.DeletedAt)
            .IsRequired(false);

        // Global Query Filter para soft delete
        builder.HasQueryFilter(e => e.IsDeleted == false);

        // Índice para consultas de soft delete
        builder.HasIndex(e => e.IsDeleted);
    }
}