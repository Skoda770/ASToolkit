using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASToolkit.Infrastructure.Interfaces;

public interface IDbContext
{
    DatabaseFacade Database { get; }
    Task RunMigrationsAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    int SaveChanges();

    int SaveChanges(bool acceptAllChangesOnSuccess);

    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task SeedInitial();
    
}