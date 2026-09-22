using Microsoft.AspNetCore.Mvc;
using Moq;
using OmniStaff.Api.Controllers;
using OmniStaff.Application.Dtos;
using OmniStaff.Application.Interfaces;
using Xunit;

namespace OmniStaff.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var mockAuth = new Mock<IAuthService>();
        mockAuth
            .Setup(s => s.LoginAsync("user@example.com", "correct-password"))
            .ReturnsAsync(new LoginResponse("fake-jwt", DateTime.UtcNow.AddHours(1), "Test User"));

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Login(new LoginRequest("user@example.com", "correct-password"));

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("fake-jwt", response.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var mockAuth = new Mock<IAuthService>();
        mockAuth
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((LoginResponse?)null);

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Login(new LoginRequest("user@example.com", "wrong-password"));

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
