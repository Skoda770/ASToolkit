using ASToolkit.Infrastructure.Abstracts;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ASToolkit.InfrastructureTests;

public class FakeDbContext([NotNull] DbContextOptions options) : DbContextBase(options)
{
    public DbSet<FakeEntity> FakeEntities => Set<FakeEntity>();
}