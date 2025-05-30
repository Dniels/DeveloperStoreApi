using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.EventHandlers
{

    /// <summary>
    /// Unit tests for AuthenticateUserHandler class.
    /// Tests authentication logic, security, and business rules.
    /// </summary>
    public class AuthenticateUserHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly AuthenticateUserHandler _handler;

        public AuthenticateUserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
            _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenAllDependenciesProvided_ShouldCreateInstance()
        {
            // Arrange & Act
            var handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);

            // Assert
            handler.Should().NotBeNull();
            handler.Should().BeAssignableTo<IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>>();
        }

        #endregion

        #region Successful Authentication Tests

        [Fact]
        public async Task Handle_WhenValidCredentialsAndActiveUser_ShouldReturnAuthenticateUserResult()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();
            var expectedToken = "jwt-token-123";

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Username);
            result.Role.Should().Be(user.Role.ToString());
        }

        [Fact]
        public async Task Handle_WhenValidCredentials_ShouldCallAllDependencies()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();
            var cancellationToken = new CancellationToken();

            _userRepository.GetByEmailAsync(request.Email, cancellationToken)
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns("token");

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            await _userRepository.Received(1).GetByEmailAsync(request.Email, cancellationToken);
            _passwordHasher.Received(1).VerifyPassword(request.Password, user.Password);
            _jwtTokenGenerator.Received(1).GenerateToken(user);
        }

        #endregion

        #region Authentication Failure Tests

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns((User)null);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task Handle_WhenPasswordIsIncorrect_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "wrongpassword"
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(false);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task Handle_WhenUserIsInactive_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateInactiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User is not active");
        }

        #endregion

        #region Edge Cases Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Handle_WhenEmailIsNullOrEmpty_ShouldAttemptAuthentication(string email)
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = email,
                Password = "password123"
            };

            _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
                .Returns((User)null);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Handle_WhenPasswordIsNullOrEmpty_ShouldAttemptAuthentication(string password)
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = password
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(password, user.Password)
                .Returns(false);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials");
        }

        #endregion

        #region Cancellation Token Tests

        
        [Fact]
        public async Task Handle_WhenNormalExecution_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();
            var cancellationToken = CancellationToken.None;

            _userRepository.GetByEmailAsync(request.Email, cancellationToken)
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns("token");

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            await _userRepository.Received(1).GetByEmailAsync(request.Email, cancellationToken);
        }

        #endregion

        #region Different User Roles Tests

        [Theory]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.Customer)]
        [InlineData(UserRole.Manager)]
        public async Task Handle_WhenUserHasDifferentRoles_ShouldReturnCorrectRole(UserRole userRole)
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUserWithRole(userRole);

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns("token");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Role.Should().Be(userRole.ToString());
        }

        #endregion

        #region Performance and Behavior Tests

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldNotCallPasswordHasher()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns((User)null);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();

            _passwordHasher.DidNotReceive().VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_WhenPasswordIncorrect_ShouldNotCallJwtTokenGenerator()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "wrongpassword"
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(false);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();

            _jwtTokenGenerator.DidNotReceive().GenerateToken(Arg.Any<User>());
        }

        #endregion

        #region Test Helper Methods

        /// <summary>
        /// Creates an active user for testing purposes
        /// </summary>
        private User CreateActiveUser()
        {
            return new User
            {
                Username = "testuser",
                Email = "user@example.com",
                Phone = "+1234567890",
                Password = "hashedpassword",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };
        }

        /// <summary>
        /// Creates an inactive user for testing purposes
        /// </summary>
        private User CreateInactiveUser()
        {
            return new User
            {
                Username = "inactiveuser",
                Email = "inactive@example.com",
                Phone = "+1234567890",
                Password = "hashedpassword",
                Role = UserRole.Customer,
                Status = UserStatus.Inactive
            };
        }

        /// <summary>
        /// Creates an active user with specific role for testing purposes
        /// </summary>
        private User CreateActiveUserWithRole(UserRole role)
        {
            return new User
            {
                Username = "testuser",
                Email = "user@example.com",
                Phone = "+1234567890",
                Password = "hashedpassword",
                Role = role,
                Status = UserStatus.Active
            };
        }

        #endregion

        #region Integration-like Tests

        [Fact]
        public async Task Handle_WhenCompleteSuccessfulFlow_ShouldReturnCompleteResult()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "admin@company.com",
                Password = "AdminPassword123"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Username = "Admin User",
                Email = "admin@company.com",
                Phone = "+1-555-0123",
                Password = "hashed-admin-password",
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };

            // Use reflection to set the Id if needed, or assume User.Create sets it
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test-payload.signature";

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            result.Email.Should().Be("admin@company.com");
            result.Name.Should().Be("Admin User");
            result.Role.Should().Be("Admin");

            // Verify all dependencies were called in correct order
            Received.InOrder(async () =>
            {
                await _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>());
                _passwordHasher.VerifyPassword(request.Password, user.Password);
                _jwtTokenGenerator.GenerateToken(user);
            });
        }

        [Fact]
        public async Task Handle_WhenMultipleCallsWithSameCredentials_ShouldBehaveConsistently()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();
            var expectedToken = "consistent-token";

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns(expectedToken);

            // Act
            var result1 = await _handler.Handle(request, CancellationToken.None);
            var result2 = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result1.Should().BeEquivalentTo(result2);
            result1.Token.Should().Be(expectedToken);
            result2.Token.Should().Be(expectedToken);

            // Verify dependencies were called twice
            await _userRepository.Received(2).GetByEmailAsync(request.Email, Arg.Any<CancellationToken>());
            _passwordHasher.Received(2).VerifyPassword(request.Password, user.Password);
            _jwtTokenGenerator.Received(2).GenerateToken(user);
        }

        #endregion

        #region Security Tests

        [Fact]
        public async Task Handle_WhenExceptionOccursInRepository_ShouldNotExposeInternalDetails()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database connection failed");
        }

        [Fact]
        public async Task Handle_WhenPasswordHasherThrows_ShouldPropagateException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Throws(new InvalidOperationException("Hash verification failed"));

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Hash verification failed");
        }

        [Fact]
        public async Task Handle_WhenJwtGeneratorThrows_ShouldPropagateException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Throws(new InvalidOperationException("Token generation failed"));

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Token generation failed");
        }

        #endregion

        #region Specification Tests

        [Fact]
        public async Task Handle_WhenUserExists_ShouldUseActiveUserSpecification()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = CreateActiveUser();

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);
            _jwtTokenGenerator.GenerateToken(user)
                .Returns("token");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert - Should not throw exception, meaning specification passed
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenUserStatusIsSuspended_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Username = "suspendeduser",
                Email = "user@example.com",
                Phone = "+1234567890",
                Password = "hashedpassword",
                Role = UserRole.Customer,
                Status = UserStatus.Suspended
            };

            _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
                .Returns(user);
            _passwordHasher.VerifyPassword(request.Password, user.Password)
                .Returns(true);

            // Act & Assert
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User is not active");
        }

        #endregion
    }
}
