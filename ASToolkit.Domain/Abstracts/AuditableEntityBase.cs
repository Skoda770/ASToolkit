using System.ComponentModel.DataAnnotations;
using ASToolkit.Domain.Interfaces;

namespace ASToolkit.Domain.Abstracts;
public abstract class AuditableEntityBase<TKey> : IAuditableEntity
{
    [Key] public TKey Uid { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }

    public void MarkUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
    public void MarkDeleted() => DeletedAt = DateTimeOffset.UtcNow;

    public override bool Equals(object? obj)
    {
        if (obj is not AuditableEntityBase<TKey> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return EqualityComparer<TKey>.Default.Equals(Uid, other.Uid);
    }

    public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Uid!);
}