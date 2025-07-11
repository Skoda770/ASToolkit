using FluentAssertions;

namespace ASToolkit.DomainTests.Abstracts;


public class AuditableEntityBaseTest
{
    [Fact]
    public void CreatedAt_ShouldBeSetOnInitialization()
    {
        var entity = new FakeEntity();
        
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void MarkModified_ShouldSetModifiedAt()
    {
        var entity = new FakeEntity();
        entity.MarkUpdated();

        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkDeleted_ShouldSetDeletedAt()
    {
        var entity = new FakeEntity();
        entity.MarkDeleted();

        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Id_ShouldBeSetInConstructor()
    {
        var entity = new FakeEntity();
        entity.Uid.Should().NotBeEmpty();
    }
}