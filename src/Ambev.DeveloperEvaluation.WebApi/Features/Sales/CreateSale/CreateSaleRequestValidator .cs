using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Validator for CreateSaleRequest that defines validation rules for sale creation
    /// </summary>
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        /// <summary>
        /// Initializes validation rules for CreateSaleRequest
        /// </summary>
        public CreateSaleRequestValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty()
                .WithMessage("Sale number is required")
                .MaximumLength(50)
                .WithMessage("Sale number must not exceed 50 characters");

            RuleFor(x => x.Customer)
                .NotNull()
                .WithMessage("Customer information is required")
                .SetValidator(new CustomerInfoValidator());

            RuleFor(x => x.Branch)
                .NotNull()
                .WithMessage("Branch information is required")
                .SetValidator(new BranchInfoValidator());

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required")
                .Must(items => items.Count <= 20)
                .WithMessage("Maximum 20 items allowed per sale");

            RuleForEach(x => x.Items)
                .SetValidator(new SaleItemInfoValidator());
        }
    }

    /// <summary>
    /// Validator for CustomerInfo
    /// </summary>
    public class CustomerInfoValidator : AbstractValidator<CustomerInfo>
    {
        public CustomerInfoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Customer name is required")
                .MaximumLength(100)
                .WithMessage("Customer name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Customer email is required")
                .EmailAddress()
                .WithMessage("Customer email must be a valid email address");
        }
    }

    /// <summary>
    /// Validator for BranchInfo
    /// </summary>
    public class BranchInfoValidator : AbstractValidator<BranchInfo>
    {
        public BranchInfoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Branch ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Branch name is required")
                .MaximumLength(100)
                .WithMessage("Branch name must not exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Branch address is required")
                .MaximumLength(200)
                .WithMessage("Branch address must not exceed 200 characters");
        }
    }

    /// <summary>
    /// Validator for SaleItemInfo
    /// </summary>
    public class SaleItemInfoValidator : AbstractValidator<SaleItemInfo>
    {
        public SaleItemInfoValidator()
        {
            RuleFor(x => x.Product)
                .NotNull()
                .WithMessage("Product information is required")
                .SetValidator(new ProductInfoValidator());

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
    /// Validator for ProductInfo
    /// </summary>
    public class ProductInfoValidator : AbstractValidator<ProductInfo>
    {
        public ProductInfoValidator()
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