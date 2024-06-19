using Domain.Entities.Base.ByUser;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Mappings;

public abstract class EntityByUserMapping<TEntity> : EntityMapping<TEntity>
    where TEntity : EntityByUser
{
    protected override void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedByUserId);
        builder.Property(x => x.UpdatedByUserId);

        builder.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedByUserId);
        builder.HasOne(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedByUserId);

        ConfigureEntityByUser(builder);
    }

    protected abstract void ConfigureEntityByUser(EntityTypeBuilder<TEntity> builder);
}