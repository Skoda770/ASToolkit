namespace ASToolkit.Domain.Interfaces;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? UpdatedAt { get; }
    public DateTimeOffset? DeletedAt { get; }
    void MarkUpdated();
    void MarkDeleted();
}