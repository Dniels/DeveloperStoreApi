using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Queries;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sale operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The logger instance</param>
    public SalesController(IMediator mediator, IMapper mapper, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all sales with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="size">Page size (default: 10)</param>
    /// <param name="order">Sort order</param>
    /// <param name="customerName">Filter by customer name</param>
    /// <param name="branchName">Filter by branch name</param>
    /// <param name="minDate">Filter by minimum date</param>
    /// <param name="maxDate">Filter by maximum date</param>
    /// <param name="isCancelled">Filter by cancellation status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sales</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<PagedResult<SaleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSales(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? order = null,
        [FromQuery] string? customerName = null,
        [FromQuery] string? branchName = null,
        [FromQuery] DateTime? minDate = null,
        [FromQuery] DateTime? maxDate = null,
        [FromQuery] bool? isCancelled = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetSalesQuery(page, size, order, customerName, branchName, minDate, maxDate, isCancelled);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponseWithData<PagedResult<SaleDto>>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Retrieves a sale by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    Detail = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            });

        try
        {
            var query = new GetSaleByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found"
                });

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<GetSaleResponse>(result)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    Detail = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            });

        try
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Created($"api/sales/{result.Id}", new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(result)
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Erro interno ao criar venda: {Mensagem}", ex.Message);

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid input data",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "Request",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (Application.Exceptions.ApplicationException ex)
        {
            _logger.LogError(ex, "Business rule violation while creating sale: {Mensagem}", ex.Message);

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Business rule violation",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "BusinessRule",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado em CreateSale");

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "Erro interno: " + ex.Message,
                Errors = new List<ValidationErrorDetail>
        {
            new ValidationErrorDetail
            {
                Detail = "Exception",
                Error = ex.ToString()
            }
        }
            });
        }

    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">The unique identifier of the sale to update</param>
    /// <param name="request">The sale update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    Detail = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            });

        try
        {
            var command = _mapper.Map<UpdateSaleCommand>((id, request));
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateSaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = _mapper.Map<UpdateSaleResponse>(result)
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "Id",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid input data",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "Request",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Business rule violation",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "BusinessRule",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Cancels a sale by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale to cancel</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the sale was cancelled</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var request = new CancelSaleRequest { Id = id };
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    Detail = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            });

        try
        {
            var command = new CancelSaleCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Sale cancelled successfully"
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found",
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Detail = "Id",
                        Error = ex.Message
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling sale {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
/// Cancel a specific item in a sale
/// </summary>
/// <param name="saleId">The unique identifier of the sale</param>
/// <param name="productId">The unique identifier of the product to cancel</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Success response if the sale item was cancelled</returns>
[HttpDelete("{saleId:guid}/items/{productId:guid}")]
[ProducesResponseType(typeof(ApiResponseWithData<ApiResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> CancelSaleItem(Guid saleId, Guid productId, CancellationToken cancellationToken = default)
{
    try
    {
        var command = new CancelSaleItemCommand(saleId, productId);
        await _mediator.Send(command, cancellationToken);

        var apiResponse = new ApiResponse
        {
            Success = true,
            Message = "Sale item cancelled successfully"
        };

        // DEBUG: Logs para verificar o que está sendo enviado
        _logger.LogInformation($"DEBUG - Response Success: {apiResponse.Success}");
        _logger.LogInformation($"DEBUG - Response Message: '{apiResponse.Message}'");
        _logger.LogInformation($"DEBUG - Response Message Length: {apiResponse.Message.Length}");

        // CORREÇÃO: Usar o método Ok<T> customizado do BaseController
        return Ok(apiResponse);
    }
    catch (NotFoundException ex)
    {
        return NotFound(new ApiResponse
        {
            Success = false,
            Message = "Sale or item not found",
            Errors = new List<ValidationErrorDetail>
            {
                new ValidationErrorDetail
                {
                    Detail = "SaleId/ProductId",
                    Error = ex.Message
                }
            }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error cancelling sale item. SaleId: {SaleId}, ProductId: {ProductId}", saleId, productId);
        return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
        {
            Success = false,
            Message = "An error occurred while processing your request"
        });
    }
}
}