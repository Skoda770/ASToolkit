using System.Linq.Expressions;
using ASToolkit.Domain.Abstracts;
using ASToolkit.Domain.Interfaces;
using ASToolkit.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASToolkit.Infrastructure.Abstracts;

public abstract class DbContextBase(DbContextOptions options) : DbContext(options), IDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (InheritsFromGenericAuditable(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    CreateDeletedAtFilter(entityType.ClrType)
                );
                modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(IAuditableEntity.CreatedAt));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static bool InheritsFromGenericAuditable(Type type)
    {
        while (type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AuditableEntityBase<>))
                return true;
            type = type.BaseType!;
        }

        return false;
    }

    private static LambdaExpression CreateDeletedAtFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var deletedAtProperty = Expression.Property(parameter, nameof(IAuditableEntity.DeletedAt));
        var filterExpression = Expression.Lambda(
            Expression.Equal(deletedAtProperty, Expression.Constant(null)),
            parameter
        );
        return filterExpression;
    }

    public Task RunMigrationsAsync()
    {
        if (Database.IsInMemory()) return Task.CompletedTask;

        var migrations = Database.GetPendingMigrations().ToList();
        return migrations.Count != 0 ? Database.MigrateAsync() : Task.CompletedTask;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await Database.BeginTransactionAsync();

    public virtual Task SeedInitial() => Task.CompletedTask;

    private void HandleEntries()
    {
        ChangeTracker.DetectChanges();
        var entries = ChangeTracker.Entries().ToList();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    HandleAddedEntry(entry);
                    break;

                case EntityState.Modified:
                    HandleModifiedEntry(entry);
                    break;

                case EntityState.Deleted:
                    HandleDeletedEntry(entry);
                    break;
            }
        }
    }

    public override int SaveChanges()
    {
        HandleEntries();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleEntries();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleEntries();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        HandleEntries();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected virtual void HandleAddedEntry(EntityEntry entry)
    {
        
    }

    protected virtual void HandleModifiedEntry(EntityEntry entry)
    {
        if (entry.Entity is IAuditableEntity model)
        {
            model.MarkUpdated();
        }
    }

    protected virtual void HandleDeletedEntry(EntityEntry entry)
    {
        if (entry.Entity is IAuditableEntity model)
        {
            model.MarkDeleted();
            entry.State = EntityState.Modified;
        }
    }
}