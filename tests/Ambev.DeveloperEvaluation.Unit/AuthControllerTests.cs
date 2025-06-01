using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AuthController(_mediatorMock.Object, _mapperMock.Object);
        }

        #region AuthenticateUser Tests

        [Fact]
        public async Task AuthenticateUser_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "!12345aA"
            };

            var command = new AuthenticateUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var commandResult = new AuthenticateUserResult
            {
                 Token = "valid-jwt-token",
                 Id = Guid.NewGuid(),
                 Email = "test@example.com",
                 Name = "TestUser"
            };

            var response = new AuthenticateUserResponse
            {
                Token = commandResult.Token,
                Name = commandResult.Name,
                Email = commandResult.Email,
                Role = commandResult.Role
            };

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(commandResult)).Returns(response);

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<WebApi.Common.ApiResponseWithData<WebApi.Common.ApiResponseWithData<AuthenticateUserResponse>>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("User authenticated successfully", apiResponse.Data.Message);
            Assert.Equal(response.Token, apiResponse.Data.Data.Token);
            Assert.Equal(response.Email, apiResponse.Data.Data.Email);
            Assert.Equal(response.Name, apiResponse.Data.Data.Name);
        }

        [Fact]
        public async Task AuthenticateUser_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticateUserRequest(); // Invalid request (empty email/password)

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("", "!12345aA")] // Empty email
        [InlineData("invalid-email", "!12345aA")] // Invalid email format
        [InlineData("test@example.com", "")] // Empty password
        public async Task AuthenticateUser_InvalidFieldValues_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = email,
                Password = password
            };

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AuthenticateUser_MediatorThrowsUnauthorizedException_PropagatesException()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword123"
            };

            var command = new AuthenticateUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _controller.AuthenticateUser(request, CancellationToken.None));
        }

        [Fact]
        public async Task AuthenticateUser_MediatorThrowsGenericException_PropagatesException()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "!12345aA"
            };

            var command = new AuthenticateUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _controller.AuthenticateUser(request, CancellationToken.None));
        }

        #endregion

        #region Mapper and Mediator Verification Tests

        [Fact]
        public async Task AuthenticateUser_ValidRequest_CallsMapperAndMediatorCorrectly()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "!12345aA"
            };

            var command = new AuthenticateUserCommand();
            var commandResult = new AuthenticateUserResult
            {
                Token = "test-token",
                Id = Guid.NewGuid()
            };
            var response = new AuthenticateUserResponse
            {
                Token = "test-token"
            };

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(commandResult)).Returns(response);

            // Act
            await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<AuthenticateUserCommand>(request), Times.Once);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<AuthenticateUserResponse>(commandResult), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUser_ValidRequest_UsesCancellationToken()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "!12345aA"
            };

            var command = new AuthenticateUserCommand();
            var commandResult = new AuthenticateUserResult
            {
                Token = "test-token",
                Id = Guid.NewGuid()
            };
            var response = new AuthenticateUserResponse();

            var cancellationToken = new CancellationToken();

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, cancellationToken)).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(commandResult)).Returns(response);

            // Act
            await _controller.AuthenticateUser(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, cancellationToken), Times.Once);
        }

        #endregion

        #region Response Structure Tests

        [Fact]
        public async Task AuthenticateUser_SuccessfulAuthentication_ReturnsCorrectResponseStructure()
        {
            // Arrange
            var request = new AuthenticateUserRequest
            {
                Email = "test@example.com",
                Password = "!12345aA"
            };

            var command = new AuthenticateUserCommand();
            var commandResult = new AuthenticateUserResult
            {
                Token = "jwt-token-123",
                Name = "TestUser",
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = UserRole.Customer.ToString()
                
            };

            var response = new AuthenticateUserResponse
            {
                Name = commandResult.Name,
                Email = commandResult.Email,
                Token = commandResult.Token,
                Role = commandResult.Role
            };

            _mapperMock.Setup(m => m.Map<AuthenticateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<AuthenticateUserResponse>(commandResult)).Returns(response);

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<WebApi.Common.ApiResponseWithData<WebApi.Common.ApiResponseWithData<AuthenticateUserResponse>>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("User authenticated successfully", apiResponse.Data.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(response.Token, apiResponse.Data.Data.Token);
        }

        #endregion
    }
}
