using System;
using ASToolkit.Domain.Abstracts;

namespace ASToolkit.InfrastructureTests;

public class FakeEntity : AuditableEntityBase<Guid>
{
    public string Name { get; set; } = null!;

}