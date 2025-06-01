using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    /// <summary>
    /// Validator for UpdateSaleRequest that defines validation rules for sale update
    /// </summary>
    public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
    {
        /// <summary>
        /// Initializes validation rules for UpdateSaleRequest
        /// </summary>
        public UpdateSaleRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required")
                .Must(items => items.Count <= 20)
                .WithMessage("Maximum 20 items allowed per sale");

            RuleForEach(x => x.Items)
                .SetValidator(new UpdateSaleItemRequestValidator());
        }
    }

    /// <summary>
    /// Validator for UpdateSaleItemRequest
    /// </summary>
    public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
    {
        public UpdateSaleItemRequestValidator()
        {
            RuleFor(x => x.Product)
                .NotNull()
                .WithMessage("Product information is required")
                .SetValidator(new UpdateProductRequestValidator());

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(1000)
                .WithMessage("Quantity cannot exceed 1000");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0")
                .LessThanOrEqualTo(999999.99m)
                .WithMessage("Unit price cannot exceed 999,999.99");
        }
    }

    /// <summary>
    /// Validator for UpdateProductRequest
    /// </summary>
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required")
                .MaximumLength(100)
                .WithMessage("Product name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Product description must not exceed 500 characters");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Product category is required")
                .MaximumLength(50)
                .WithMessage("Product category must not exceed 50 characters");
        }
    }
}