using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Validators
{
    /// <summary>
    /// Unit tests for AuthenticateUserValidator class.
    /// Tests validation rules for email and password fields in authentication commands.
    /// </summary>
    public class AuthenticateUserValidatorTests
    {
        private readonly AuthenticateUserValidator _validator;

        public AuthenticateUserValidatorTests()
        {
            _validator = new AuthenticateUserValidator();
        }

        #region Email Validation Tests

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = string.Empty,
                Password = "ValidPass123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public void Validate_WhenEmailIsInvalidFormat_ShouldReturnValidationError()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "invalid-email",
                Password = "ValidPass123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public void Validate_WhenEmailIsValid_ShouldNotReturnEmailValidationError()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "ValidPass123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == nameof(command.Email));
        }

        #endregion

        #region Password Validation Tests

        [Fact]
        public void Validate_WhenPasswordIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = string.Empty
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345")]
        public void Validate_WhenPasswordIsLessThanSixCharacters_ShouldReturnValidationError(string shortPassword)
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = shortPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("ValidPassword123")]
        public void Validate_WhenPasswordIsSixOrMoreCharacters_ShouldNotReturnPasswordValidationError(string validPassword)
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = validPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == nameof(command.Password));
        }

        #endregion

        #region Combined Validation Tests

        [Fact]
        public void Validate_WhenBothEmailAndPasswordAreValid_ShouldReturnValid()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "user@example.com",
                Password = "ValidPass123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenBothEmailAndPasswordAreInvalid_ShouldReturnMultipleValidationErrors()
        {
            // Arrange
            var command = new AuthenticateUserCommand
            {
                Email = "invalid-email",
                Password = "123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        #endregion
    }
}

