namespace Domain.Entities.Base;

public abstract class Entity
{
    protected Entity()
    {
        SetId(Guid.NewGuid());
        SetCreatedAt(DateTime.UtcNow);
    }

    public DateTime CreatedAt { get; private set; }
    public Guid Id { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static bool operator !=(Entity entityA, Entity entityB)
        => !(entityA == entityB);

    public static bool operator ==(Entity entityA, Entity entityB)
        => entityA?.Equals(entityB) ?? entityB is null;

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() != GetType())
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (Id == ((Entity)obj).Id)
            return true;

        return false;
    }

    public override int GetHashCode()
        => Id.GetHashCode();

    public Entity SetUpdatedAt(DateTime? updatedAt)
    {
        UpdatedAt = updatedAt;
        return this;
    }

    public override string ToString() => $"{GetType().Name}[{Id}]";

    private void SetCreatedAt(DateTime createdAt)
    {
        CreatedAt = createdAt;
    }

    private void SetId(Guid id)
    {
        Id = id;
    }
}