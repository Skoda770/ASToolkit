using System;
using System.ComponentModel.DataAnnotations;
using ASToolkit.Domain.Abstracts;

namespace ASToolkit.InfrastructureTests;

public class FakeEntity : AuditableEntityBase<Guid>
{
    [MaxLength(256)]
    public string Name { get; set; } = null!;

}