using System;
using System.Threading.Tasks;
using ASToolkit.Auth.DTO;
using ASToolkit.Auth.Interfaces;
using ASToolkit.Auth.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.AuthTests.Services;

[TestSubject(typeof(IdentityService))]
public class IdentityServiceTest
{
    private readonly IdentityService _service;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService = new Mock<IJwtTokenService>();
    private readonly Mock<ILogger<IdentityService>> _mockLogger = new Mock<ILogger<IdentityService>>();

    public IdentityServiceTest()
    {
        var dbContext = TestDbContextFactory.CreateDbInMemory();
        dbContext.SeedUsers();

        _service = new IdentityService(null, null, _mockJwtTokenService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task LogoutAsync_ShouldNotThrowWhenUserIdIsInvalid()
    {
        var userId = Guid.Empty;
        await _service.LogoutAsync(userId);

        _mockJwtTokenService.Verify(s => s.ClearUserTokensAsync(userId), Times.Once);
    }

    [Fact]
    public void Logout_ShouldNotThrowWhenUserIdIsInvalid()
    {
        var userId = Guid.Empty;

        _service.Logout(userId);

        _mockJwtTokenService.Verify(s => s.ClearUserTokens(userId), Times.Once);
    }
}