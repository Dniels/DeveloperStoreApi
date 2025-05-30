using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Results
{
    /// <summary>
    /// Unit tests for AuthenticateUserResult class.
    /// Tests property assignments, default values, and result structure.
    /// </summary>
    public class AuthenticateUserResultTests
    {
        #region Property Tests

        [Fact]
        public void Token_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";

            // Act
            result.Token = expectedToken;

            // Assert
            result.Token.Should().Be(expectedToken);
        }

        [Fact]
        public void Id_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedId = Guid.NewGuid();

            // Act
            result.Id = expectedId;

            // Assert
            result.Id.Should().Be(expectedId);
        }

        [Fact]
        public void Name_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedName = "John Doe";

            // Act
            result.Name = expectedName;

            // Assert
            result.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Email_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedEmail = "john.doe@example.com";

            // Act
            result.Email = expectedEmail;

            // Assert
            result.Email.Should().Be(expectedEmail);
        }

        [Fact]
        public void Phone_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedPhone = "+1234567890";

            // Act
            result.Phone = expectedPhone;

            // Assert
            result.Phone.Should().Be(expectedPhone);
        }

        [Fact]
        public void Role_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var expectedRole = "Admin";

            // Act
            result.Role = expectedRole;

            // Assert
            result.Role.Should().Be(expectedRole);
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void Token_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Token.Should().Be(string.Empty);
        }

        [Fact]
        public void Id_DefaultValue_ShouldBeEmptyGuid()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Name_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Name.Should().Be(string.Empty);
        }

        [Fact]
        public void Email_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Email.Should().Be(string.Empty);
        }

        [Fact]
        public void Phone_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Phone.Should().Be(string.Empty);
        }

        [Fact]
        public void Role_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Role.Should().Be(string.Empty);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var result = new AuthenticateUserResult();

            // Assert
            result.Token.Should().Be(string.Empty);
            result.Id.Should().Be(Guid.Empty);
            result.Name.Should().Be(string.Empty);
            result.Email.Should().Be(string.Empty);
            result.Phone.Should().Be(string.Empty);
            result.Role.Should().Be(string.Empty);
        }

        #endregion

        #region Object Initialization Tests

        [Fact]
        public void Result_WhenInitializedWithObjectInitializer_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var expectedToken = "test-jwt-token";
            var expectedId = Guid.NewGuid();
            var expectedName = "Jane Smith";
            var expectedEmail = "jane.smith@example.com";
            var expectedPhone = "+1987654321";
            var expectedRole = "User";

            // Act
            var result = new AuthenticateUserResult
            {
                Token = expectedToken,
                Id = expectedId,
                Name = expectedName,
                Email = expectedEmail,
                Phone = expectedPhone,
                Role = expectedRole
            };

            // Assert
            result.Token.Should().Be(expectedToken);
            result.Id.Should().Be(expectedId);
            result.Name.Should().Be(expectedName);
            result.Email.Should().Be(expectedEmail);
            result.Phone.Should().Be(expectedPhone);
            result.Role.Should().Be(expectedRole);
        }

        [Theory]
        [InlineData("Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", "Admin")]
        [InlineData("jwt-token-123", "User")]
        [InlineData("", "Guest")]
        public void Result_WhenInitializedWithDifferentTokensAndRoles_ShouldRetainValues(string token, string role)
        {
            // Arrange & Act
            var result = new AuthenticateUserResult
            {
                Token = token,
                Role = role
            };

            // Assert
            result.Token.Should().Be(token);
            result.Role.Should().Be(role);
        }

        #endregion

        #region Null Values Tests

        [Fact]
        public void Token_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Token = null;

            // Assert
            result.Token.Should().BeNull();
        }

        [Fact]
        public void Name_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Name = null;

            // Assert
            result.Name.Should().BeNull();
        }

        [Fact]
        public void Email_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Email = null;

            // Assert
            result.Email.Should().BeNull();
        }

        [Fact]
        public void Phone_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Phone = null;

            // Assert
            result.Phone.Should().BeNull();
        }

        [Fact]
        public void Role_WhenSetToNull_ShouldAcceptNullValue()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Role = null;

            // Assert
            result.Role.Should().BeNull();
        }

        #endregion

        #region Special Characters and Edge Cases

        [Theory]
        [InlineData("João Silva", "joão@empresa.com.br", "+55 11 99999-9999")]
        [InlineData("María García", "maría@compañía.es", "+34 123 456 789")]
        [InlineData("张三", "zhangsan@company.cn", "+86 138 0013 8000")]
        public void Result_WhenSetWithInternationalCharacters_ShouldHandleCorrectly(string name, string email, string phone)
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Name = name;
            result.Email = email;
            result.Phone = phone;

            // Assert
            result.Name.Should().Be(name);
            result.Email.Should().Be(email);
            result.Phone.Should().Be(phone);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void StringProperties_WhenSetToWhitespaceOrEmpty_ShouldRetainValue(string value)
        {
            // Arrange
            var result = new AuthenticateUserResult();

            // Act
            result.Token = value;
            result.Name = value;
            result.Email = value;
            result.Phone = value;
            result.Role = value;

            // Assert
            result.Token.Should().Be(value);
            result.Name.Should().Be(value);
            result.Email.Should().Be(value);
            result.Phone.Should().Be(value);
            result.Role.Should().Be(value);
        }

        #endregion

        #region Multiple Assignment Tests

        [Fact]
        public void Properties_WhenSetMultipleTimes_ShouldRetainLatestValues()
        {
            // Arrange
            var result = new AuthenticateUserResult();

            var initialToken = "initial-token";
            var finalToken = "final-token";
            var initialName = "Initial Name";
            var finalName = "Final Name";
            var initialId = Guid.NewGuid();
            var finalId = Guid.NewGuid();

            // Act
            result.Token = initialToken;
            result.Name = initialName;
            result.Id = initialId;

            result.Token = finalToken;
            result.Name = finalName;
            result.Id = finalId;

            // Assert
            result.Token.Should().Be(finalToken);
            result.Name.Should().Be(finalName);
            result.Id.Should().Be(finalId);
        }

        #endregion

        #region Type Safety Tests

        [Fact]
        public void AuthenticateUserResult_ShouldBeSealed()
        {
            // Arrange
            var resultType = typeof(AuthenticateUserResult);

            // Assert
            resultType.IsSealed.Should().BeTrue();
        }

        [Fact]
        public void Id_ShouldBeGuidType()
        {
            // Arrange
            var result = new AuthenticateUserResult();
            var propertyInfo = typeof(AuthenticateUserResult).GetProperty(nameof(result.Id));

            // Assert
            propertyInfo.PropertyType.Should().Be(typeof(Guid));
        }

        [Fact]
        public void StringProperties_ShouldAllBeStringType()
        {
            // Arrange
            var resultType = typeof(AuthenticateUserResult);
            var stringProperties = new[] { "Token", "Name", "Email", "Phone", "Role" };

            // Act & Assert
            foreach (var propertyName in stringProperties)
            {
                var propertyInfo = resultType.GetProperty(propertyName);
                propertyInfo.PropertyType.Should().Be(typeof(string),
                    $"Property {propertyName} should be of type string");
            }
        }

        #endregion

        #region Complete Result Tests

        [Fact]
        public void Result_WhenCompletelyPopulated_ShouldRepresentValidAuthenticationResult()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var id = Guid.NewGuid();
            var name = "John Doe";
            var email = "john.doe@company.com";
            var phone = "+1-555-123-4567";
            var role = "Administrator";

            // Act
            var result = new AuthenticateUserResult
            {
                Token = token,
                Id = id,
                Name = name,
                Email = email,
                Phone = phone,
                Role = role
            };

            // Assert
            result.Token.Should().NotBeNullOrEmpty();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().NotBeNullOrEmpty();
            result.Email.Should().NotBeNullOrEmpty();
            result.Phone.Should().NotBeNullOrEmpty();
            result.Role.Should().NotBeNullOrEmpty();

            // Verify specific values
            result.Token.Should().Be(token);
            result.Id.Should().Be(id);
            result.Name.Should().Be(name);
            result.Email.Should().Be(email);
            result.Phone.Should().Be(phone);
            result.Role.Should().Be(role);
        }

        #endregion
    }
}
