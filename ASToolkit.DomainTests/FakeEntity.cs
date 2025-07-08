using ASToolkit.Domain.Abstracts;

namespace ASToolkit.DomainTests;

public class FakeEntity : AuditableEntityBase<Guid>
{
    public FakeEntity()
    {
        Uid = Guid.NewGuid();
    }
}