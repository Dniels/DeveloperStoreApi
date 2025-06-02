using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using FluentAssertions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Commands
{
    /// <summary>
    /// Unit tests for CancelSaleCommand record.
    /// Tests record initialization, property values, and interface implementation.
    /// </summary>
    public class CancelSaleCommandTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidId_ShouldSetIdProperty()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            // Act
            var command = new CancelSaleCommand(expectedId);

            // Assert
            command.Id.Should().Be(expectedId);
        }

        [Fact]
        public void Constructor_WhenCalledWithEmptyGuid_ShouldSetIdToEmptyGuid()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act
            var command = new CancelSaleCommand(emptyId);

            // Assert
            command.Id.Should().Be(Guid.Empty);
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void CancelSaleCommand_ShouldImplementIRequest()
        {
            // Arrange
            var command = new CancelSaleCommand(Guid.NewGuid());

            // Assert
            command.Should().BeAssignableTo<IRequest<bool>>();
        }

        #endregion

        #region Record Equality Tests

        [Fact]
        public void CancelSaleCommand_WhenTwoInstancesHaveSameId_ShouldBeEqual()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command1 = new CancelSaleCommand(id);
            var command2 = new CancelSaleCommand(id);

            // Act & Assert
            command1.Should().Be(command2);
            command1.Equals(command2).Should().BeTrue();
            (command1 == command2).Should().BeTrue();
        }

        [Fact]
        public void CancelSaleCommand_WhenTwoInstancesHaveDifferentIds_ShouldNotBeEqual()
        {
            // Arrange
            var command1 = new CancelSaleCommand(Guid.NewGuid());
            var command2 = new CancelSaleCommand(Guid.NewGuid());

            // Act & Assert
            command1.Should().NotBe(command2);
            command1.Equals(command2).Should().BeFalse();
            (command1 != command2).Should().BeTrue();
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldContainIdValue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new CancelSaleCommand(id);

            // Act
            var result = command.ToString();

            // Assert
            result.Should().Contain(id.ToString());
        }

        #endregion
    }
}
