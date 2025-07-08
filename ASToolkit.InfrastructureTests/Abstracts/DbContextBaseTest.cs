using System;
using System.Linq;
using System.Threading.Tasks;
using ASToolkit.Domain.Interfaces;
using ASToolkit.Infrastructure.Abstracts;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASToolkit.InfrastructureTests.Abstracts;

[TestSubject(typeof(DbContextBase))]
public class DbContextBaseTest
{
    private readonly FakeDbContext _context;
    
    public DbContextBaseTest()
    {
        var options = new DbContextOptionsBuilder<FakeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new FakeDbContext(options);
    }
    [Fact]
    public async Task AddEntity_ShouldSetCreatedAt()
    {
        var entity = new FakeEntity { Name = "Test" };
        _context.FakeEntities.Add(entity);
        await _context.SaveChangesAsync();

        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task UpdateEntity_ShouldSetModifiedAt()
    {
        var entity = new FakeEntity { Name = "Before" };
        _context.FakeEntities.Add(entity);
        await _context.SaveChangesAsync();

        entity.Name = "After";
        _context.FakeEntities.Update(entity);
        await _context.SaveChangesAsync();

        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteEntity_ShouldSetDeletedAtAndNotRemoveFromDb()
    {
        var entity = new FakeEntity { Name = "ToDelete" };
        _context.FakeEntities.Add(entity);
        await _context.SaveChangesAsync();

        _context.FakeEntities.Remove(entity);
        await _context.SaveChangesAsync();

        entity.DeletedAt.Should().NotBeNull();
        _context.FakeEntities.Count().Should().Be(0);
    }

    [Fact]
    public async Task DeletedEntity_ShouldRemainInDatabase()
    {
        var entity = new FakeEntity { Name = "SoftDelete" };
        _context.FakeEntities.Add(entity);
        await _context.SaveChangesAsync();

        _context.FakeEntities.Remove(entity);
        await _context.SaveChangesAsync();

        var rawCount = await _context.FakeEntities
            .IgnoreQueryFilters()
            .CountAsync();

        rawCount.Should().Be(1);
    }
    [Fact]
    public void OnModelCreating_ShouldAddQueryFilter_AndIndex()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(FakeEntity))!;
        
        var hasFilter = entityType.GetQueryFilter() != null;
        hasFilter.Should().BeTrue();

        var hasIndex = entityType.GetIndexes()
            .Any(i => i.Properties.Any(p => p.Name == nameof(IAuditableEntity.CreatedAt)));
        hasIndex.Should().BeTrue();
    }
}