using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Queries;
using Ambev.DeveloperEvaluation.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{

    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SalesController> _logger;

        public SalesController(IMediator mediator, ILogger<SalesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all sales with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<SaleDto>>> GetSales(
            [FromQuery] int _page = 1,
            [FromQuery] int _size = 10,
            [FromQuery] string? _order = null,
            [FromQuery] string? customerName = null,
            [FromQuery] string? branchName = null,
            [FromQuery] DateTime? _minDate = null,
            [FromQuery] DateTime? _maxDate = null,
            [FromQuery] bool? isCancelled = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetSalesQuery(_page, _size, _order, customerName, branchName, _minDate, _maxDate, isCancelled);
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales");
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }

        /// <summary>
        /// Get a sale by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SaleDto>> GetSale(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetSaleByIdQuery(id);
                var result = await _mediator.Send(query, cancellationToken);

                if (result == null)
                    return NotFound(new ErrorResponse("ResourceNotFound", "Sale not found", $"The sale with ID {id} does not exist"));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale {SaleId}", id);
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }

        /// <summary>
        /// Create a new sale
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SaleDto>> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new CreateSaleCommand(
                    request.SaleNumber,
                    request.Customer.Id,
                    request.Customer.Name,
                    request.Customer.Email,
                    request.Branch.Id,
                    request.Branch.Name,
                    request.Branch.Address,
                    request.Items.Select(i => new CreateSaleItemDto(
                        i.Product.Id,
                        i.Product.Name,
                        i.Product.Description,
                        i.Product.Category,
                        i.Quantity,
                        i.UnitPrice
                    )).ToList()
                );

                var result = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetSale), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse("ValidationError", "Invalid input data", ex.Message));
            }
            catch (Application.Exceptions.ApplicationException ex)
            {
                return BadRequest(new ErrorResponse("BusinessRuleViolation", "Business rule violation", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }

        /// <summary>
        /// Update an existing sale
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SaleDto>> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var command = new UpdateSaleCommand(
                    id,
                    request.Items.Select(i => new UpdateSaleItemDto(
                        i.Product.Id,
                        i.Product.Name,
                        i.Product.Description,
                        i.Product.Category,
                        i.Quantity,
                        i.UnitPrice
                    )).ToList()
                );

                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse("ResourceNotFound", "Sale not found", ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse("ValidationError", "Invalid input data", ex.Message));
            }
            catch (DomainException ex)
            {
                return BadRequest(new ErrorResponse("BusinessRuleViolation", "Business rule violation", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale {SaleId}", id);
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }

        /// <summary>
        /// Cancel a sale
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> CancelSale(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var command = new CancelSaleCommand(id);
                await _mediator.Send(command, cancellationToken);
                return Ok(new { message = "Sale cancelled successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse("ResourceNotFound", "Sale not found", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sale {SaleId}", id);
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }

        /// <summary>
        /// Cancel a specific item in a sale
        /// </summary>
        [HttpDelete("{saleId:guid}/items/{productId:guid}")]
        public async Task<ActionResult> CancelSaleItem(Guid saleId, Guid productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var command = new CancelSaleItemCommand(saleId, productId);
                await _mediator.Send(command, cancellationToken);
                return Ok(new { message = "Sale item cancelled successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse("ResourceNotFound", "Sale or item not found", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sale item. SaleId: {SaleId}, ProductId: {ProductId}", saleId, productId);
                return StatusCode(500, new ErrorResponse("InternalServerError", "An error occurred while processing your request", ex.Message));
            }
        }
    }
}
