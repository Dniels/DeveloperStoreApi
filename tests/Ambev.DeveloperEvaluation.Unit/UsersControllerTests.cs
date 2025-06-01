using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit
{
    public class UsersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);
        }

        #region CreateUser Tests

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Phone = "1234567890",
                Password = "!12345aA",
                Status = UserStatus.Active,
                Role = UserRole.Customer
            };

            var command = new CreateUserCommand();
            var commandResult = new CreateUserResult { Id = Guid.NewGuid() };
            var response = new CreateUserResponse { Id = commandResult.Id };

            _mapperMock.Setup(m => m.Map<CreateUserCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<CreateUserResponse>(commandResult)).Returns(response);

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var apiResponse = Assert.IsType<WebApi.Common.ApiResponseWithData<CreateUserResponse>>(createdResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("User created successfully", apiResponse.Message);
            Assert.Equal(response.Id, apiResponse.Data.Id);
        }

        [Fact]
        public async Task CreateUser_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateUserRequest(); // Invalid request

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetUser Tests

        [Fact]
        public async Task GetUser_ValidId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new GetUserCommand(userId);
            var commandResult = new GetUserResult { Id = userId };
            var response = new GetUserResponse { Id = userId };

            _mapperMock.Setup(m => m.Map<GetUserCommand>(userId)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
            _mapperMock.Setup(m => m.Map<GetUserResponse>(commandResult)).Returns(response);

            // Act
            var result = await _controller.GetUser(userId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            //var apiResponse = Assert.IsType<ApiResponseWithData<GetUserResponse>>(okResult.Value);
            //Assert.True(apiResponse.Success);
            Assert.Equal("User retrieved successfully", ((Ambev.DeveloperEvaluation.WebApi.Common.ApiResponseWithData<Ambev.DeveloperEvaluation.WebApi.Common.ApiResponseWithData<Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser.GetUserResponse>>)okResult.Value).Data.Message);
            Assert.Equal(userId, ((Ambev.DeveloperEvaluation.WebApi.Common.ApiResponseWithData<Ambev.DeveloperEvaluation.WebApi.Common.ApiResponseWithData<Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser.GetUserResponse>>)okResult.Value).Data.Data.Id);
        }

        [Fact]
        public async Task GetUser_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.GetUser(invalidId, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region DeleteUser Tests

        [Fact]
        public async Task DeleteUser_ValidId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);
            var commandResult = new DeleteUserResponse { Success = true };

            _mapperMock.Setup(m => m.Map<DeleteUserCommand>(userId)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);

            // Act
            var result = await _controller.DeleteUser(userId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<WebApi.Common.ApiResponseWithData<WebApi.Common.ApiResponse>>(okResult.Value);

            Assert.NotNull(apiResponse.Data); // Ensure Data is not null
            Assert.Equal("User deleted successfully", apiResponse.Data.Message);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteUser(invalidId, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task GetUser_MediatorThrowsException_PropagatesException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new GetUserCommand(userId);

            _mapperMock.Setup(m => m.Map<GetUserCommand>(userId)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _controller.GetUser(userId, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteUser_MediatorThrowsException_PropagatesException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);

            _mapperMock.Setup(m => m.Map<DeleteUserCommand>(userId)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _controller.DeleteUser(userId, CancellationToken.None));
        }

        #endregion

        #region Validation Tests

        [Theory]
        [InlineData("", "test@example.com", "1234567890", "ValidPassword123")]
        [InlineData("testuser", "", "1234567890", "ValidPassword123")]
        [InlineData("testuser", "test@example.com", "", "ValidPassword123")]
        [InlineData("testuser", "test@example.com", "1234567890", "")]
        public async Task CreateUser_InvalidFieldValues_ReturnsBadRequest(
            string username, string email, string phone, string password)
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = username,
                Email = email,
                Phone = phone,
                Password = password
            };

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}
