using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Commands
{
    /// <summary>
    /// Unit tests for AuthenticateUserCommand class.
    /// Tests property assignments, default values, and command structure.
    /// </summary>
    public class AuthenticateUserCommandTests
    {
        #region Property Tests

        [Fact]
        public void Email_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();
            var expectedEmail = "test@example.com";

            // Act
            command.Email = expectedEmail;

            // Assert
            command.Email.Should().Be(expectedEmail);
        }

        [Fact]
        public void Password_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();
            var expectedPassword = "TestPassword123";

            // Act
            command.Password = expectedPassword;

            // Assert
            command.Password.Should().Be(expectedPassword);
        }

        [Fact]
        public void Email_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var command = new AuthenticateUserCommand();

            // Assert
            command.Email.Should().Be(string.Empty);
        }

        [Fact]
        public void Password_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var command = new AuthenticateUserCommand();

            // Assert
            command.Password.Should().Be(string.Empty);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var command = new AuthenticateUserCommand();

            // Assert
            command.Email.Should().Be(string.Empty);
            command.Password.Should().Be(string.Empty);
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void AuthenticateUserCommand_ShouldImplementIRequest()
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Assert
            command.Should().BeAssignableTo<IRequest<AuthenticateUserResult>>();
        }

        [Fact]
        public void AuthenticateUserCommand_ShouldBeOfCorrectGenericType()
        {
            // Arrange
            var commandType = typeof(AuthenticateUserCommand);

            // Assert
            commandType.GetInterfaces()
                .Should().Contain(typeof(IRequest<AuthenticateUserResult>));
        }

        #endregion

        #region Object Initialization Tests

        [Theory]
        [InlineData("user@domain.com", "password123")]
        [InlineData("admin@company.org", "AdminPass!@#")]
        [InlineData("test.user+tag@example.co.uk", "ComplexP@ssw0rd")]
        public void Command_WhenInitializedWithValues_ShouldRetainCorrectValues(string email, string password)
        {
            // Arrange & Act
            var command = new AuthenticateUserCommand
            {
                Email = email,
                Password = password
            };

            // Assert
            command.Email.Should().Be(email);
            command.Password.Should().Be(password);
        }

        [Fact]
        public void Command_WhenCreatedWithObjectInitializer_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var expectedEmail = "user@test.com";
            var expectedPassword = "TestPass123";

            // Act
            var command = new AuthenticateUserCommand
            {
                Email = expectedEmail,
                Password = expectedPassword
            };

            // Assert
            command.Email.Should().Be(expectedEmail);
            command.Password.Should().Be(expectedPassword);
            command.Should().BeAssignableTo<IRequest<AuthenticateUserResult>>();
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Email_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Act
            command.Email = null;

            // Assert
            command.Email.Should().BeNull();
        }

        [Fact]
        public void Password_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Act
            command.Password = null;

            // Assert
            command.Password.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Email_WhenSetToWhitespaceOrEmpty_ShouldRetainValue(string email)
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Act
            command.Email = email;

            // Assert
            command.Email.Should().Be(email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Password_WhenSetToWhitespaceOrEmpty_ShouldRetainValue(string password)
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Act
            command.Password = password;

            // Assert
            command.Password.Should().Be(password);
        }

        #endregion

        #region Multiple Assignment Tests

        [Fact]
        public void Properties_WhenSetMultipleTimes_ShouldRetainLatestValues()
        {
            // Arrange
            var command = new AuthenticateUserCommand();
            var initialEmail = "initial@example.com";
            var finalEmail = "final@example.com";
            var initialPassword = "InitialPass123";
            var finalPassword = "FinalPass456";

            // Act
            command.Email = initialEmail;
            command.Password = initialPassword;
            command.Email = finalEmail;
            command.Password = finalPassword;

            // Assert
            command.Email.Should().Be(finalEmail);
            command.Password.Should().Be(finalPassword);
        }

        #endregion

        #region Special Characters Tests

        [Theory]
        [InlineData("user@domain.com", "P@ssw0rd!#$%")]
        [InlineData("test+tag@example.org", "密码123")]
        [InlineData("用户@测试.com", "Contraseña123")]
        [InlineData("user@domain.com", "🚀🔐💻")]
        public void Command_WhenSetWithSpecialCharacters_ShouldHandleCorrectly(string email, string password)
        {
            // Arrange
            var command = new AuthenticateUserCommand();

            // Act
            command.Email = email;
            command.Password = password;

            // Assert
            command.Email.Should().Be(email);
            command.Password.Should().Be(password);
        }

        #endregion

        #region Long Values Tests

        [Fact]
        public void Email_WhenSetToVeryLongValue_ShouldRetainValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();
            var longEmail = new string('a', 1000) + "@" + new string('b', 1000) + ".com";

            // Act
            command.Email = longEmail;

            // Assert
            command.Email.Should().Be(longEmail);
            command.Email.Length.Should().Be(longEmail.Length);
        }

        [Fact]
        public void Password_WhenSetToVeryLongValue_ShouldRetainValue()
        {
            // Arrange
            var command = new AuthenticateUserCommand();
            var longPassword = new string('x', 10000);

            // Act
            command.Password = longPassword;

            // Assert
            command.Password.Should().Be(longPassword);
            command.Password.Length.Should().Be(10000);
        }

        #endregion
    }
}
