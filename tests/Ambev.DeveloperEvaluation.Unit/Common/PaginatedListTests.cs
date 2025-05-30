using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common
{
    public class PaginatedListTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var totalCount = 10;
            var pageNumber = 2;
            var pageSize = 5;

            // Act
            var result = new PaginatedList<string>(items, totalCount, pageNumber, pageSize);

            // Assert
            result.Should().HaveCount(3);
            result.CurrentPage.Should().Be(2);
            result.TotalPages.Should().Be(2); // Math.Ceiling(10/5) = 2
            result.PageSize.Should().Be(5);
            result.TotalCount.Should().Be(10);
            result.Should().ContainInOrder("item1", "item2", "item3");
        }

        [Fact]
        public void Constructor_WithEmptyItems_ShouldCreateEmptyList()
        {
            // Arrange
            var items = new List<string>();
            var totalCount = 0;
            var pageNumber = 1;
            var pageSize = 5;

            // Act
            var result = new PaginatedList<string>(items, totalCount, pageNumber, pageSize);

            // Assert
            result.Should().BeEmpty();
            result.CurrentPage.Should().Be(1);
            result.TotalPages.Should().Be(0);
            result.PageSize.Should().Be(5);
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public void Constructor_WithNonDivisibleTotalCount_ShouldCalculateCorrectTotalPages()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var totalCount = 13; // 13/5 = 2.6, should ceil to 3
            var pageNumber = 1;
            var pageSize = 5;

            // Act
            var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

            // Assert
            result.TotalPages.Should().Be(3); // Math.Ceiling(13/5) = 3
        }

        
        #endregion

        #region HasPrevious Property Tests

        [Fact]
        public void HasPrevious_WhenCurrentPageIsOne_ShouldReturnFalse()
        {
            // Arrange
            var items = new List<string> { "item1" };
            var result = new PaginatedList<string>(items, 10, 1, 5);

            // Act & Assert
            result.HasPrevious.Should().BeFalse();
        }

        [Fact]
        public void HasPrevious_WhenCurrentPageIsGreaterThanOne_ShouldReturnTrue()
        {
            // Arrange
            var items = new List<string> { "item1" };
            var result = new PaginatedList<string>(items, 10, 2, 5);

            // Act & Assert
            result.HasPrevious.Should().BeTrue();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(10, true)]
        public void HasPrevious_WithDifferentPageNumbers_ShouldReturnExpectedValue(int pageNumber, bool expected)
        {
            // Arrange
            var items = new List<string> { "item1" };
            var result = new PaginatedList<string>(items, 50, pageNumber, 5);

            // Act & Assert
            result.HasPrevious.Should().Be(expected);
        }

        #endregion

        #region HasNext Property Tests

        [Fact]
        public void HasNext_WhenCurrentPageIsLastPage_ShouldReturnFalse()
        {
            // Arrange
            var items = new List<string> { "item1" };
            var result = new PaginatedList<string>(items, 10, 2, 5); // 2 total pages, current is 2

            // Act & Assert
            result.HasNext.Should().BeFalse();
        }

        [Fact]
        public void HasNext_WhenCurrentPageIsNotLastPage_ShouldReturnTrue()
        {
            // Arrange
            var items = new List<string> { "item1" };
            var result = new PaginatedList<string>(items, 15, 2, 5); // 3 total pages, current is 2

            // Act & Assert
            result.HasNext.Should().BeTrue();
        }

        [Theory]
        [InlineData(1, 2, true)]  // page 1 of 2
        [InlineData(2, 2, false)] // page 2 of 2
        [InlineData(1, 3, true)]  // page 1 of 3
        [InlineData(2, 3, true)]  // page 2 of 3
        [InlineData(3, 3, false)] // page 3 of 3
        public void HasNext_WithDifferentPageScenarios_ShouldReturnExpectedValue(int currentPage, int totalPages, bool expected)
        {
            // Arrange
            var items = new List<string> { "item1" };
            var totalCount = totalPages * 5; // Assuming page size of 5
            var result = new PaginatedList<string>(items, totalCount, currentPage, 5);

            // Act & Assert
            result.HasNext.Should().Be(expected);
        }

        #endregion


        #region Edge Cases and Integration Tests

        [Fact]
        public void PaginatedList_ShouldInheritFromList()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var result = new PaginatedList<string>(items, 2, 1, 5);

            // Act & Assert
            result.Should().BeAssignableTo<List<string>>();
            result[0].Should().Be("item1");
            result[1].Should().Be("item2");
        }

        [Fact]
        public void PaginatedList_ShouldSupportListOperations()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var result = new PaginatedList<string>(items, 2, 1, 5);

            // Act
            result.Add("item3");

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain("item3");
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void Constructor_WithInvalidParameters_ShouldStillCreateObject(int pageNumber, int pageSize)
        {
            // Arrange
            var items = new List<string> { "item1" };

            // Act
            var result = new PaginatedList<string>(items, 1, pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void NavigationProperties_ShouldWorkCorrectlyForSinglePageScenario()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var result = new PaginatedList<string>(items, 2, 1, 10); // Only 1 page total

            // Act & Assert
            result.HasPrevious.Should().BeFalse();
            result.HasNext.Should().BeFalse();
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public void NavigationProperties_ShouldWorkCorrectlyForMiddlePage()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var result = new PaginatedList<string>(items, 25, 3, 5); // Page 3 of 5

            // Act & Assert
            result.HasPrevious.Should().BeTrue();
            result.HasNext.Should().BeTrue();
            result.TotalPages.Should().Be(5);
        }

        #endregion

        #region Custom Test Data Classes

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public TestEntity(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

      #endregion
    }
}
