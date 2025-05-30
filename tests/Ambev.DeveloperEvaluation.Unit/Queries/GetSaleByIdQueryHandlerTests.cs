using Ambev.DeveloperEvaluation.Application.Handlers;
using Ambev.DeveloperEvaluation.Application.Queries;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Builders;
using Ambev.DeveloperEvaluation.Unit.Common;
using AutoMapper;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Queries
{
    public class GetSaleByIdQueryHandlerTests : TestBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly GetSaleByIdQueryHandler _handler;

        public GetSaleByIdQueryHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new GetSaleByIdQueryHandler(_saleRepository, Mapper);
        }

        [Fact]
        public async Task Handle_ExistingSale_ShouldReturnSaleDto()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var sale = new SaleBuilder()
                .WithId(Guid.NewGuid())
                .WithItem(Guid.NewGuid(), "Product 1", 2, 10.0m)
                .Build();

            _saleRepository.GetByIdAsync(saleId).Returns(sale);

            var query = new GetSaleByIdQuery (saleId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sale.Id, result.Id);
            Assert.Equal(sale.SaleNumber, result.SaleNumber);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task Handle_NonExistingSale_ShouldReturnNull()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            _saleRepository.GetByIdAsync(saleId).Returns((Sale)null);

            var query = new GetSaleByIdQuery (saleId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
