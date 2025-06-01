using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.WebApi.Models
{
    public class CreateRequest
    {
        [Required]
        public string SaleNumber { get; set; } = string.Empty;

        [Required]
        public CustomerRequest Customer { get; set; } = new();

        [Required]
        public BranchRequest Branch { get; set; } = new();

        [Required]
        [MinLength(1)]
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class UpdateSaleRequest
    {
        [Required]
        [MinLength(1)]
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class SaleItemRequest
    {
        [Required]
        public ProductRequest Product { get; set; } = new();

        [Required]
        [Range(1, 20)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class CustomerRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(300)]
        public string Email { get; set; } = string.Empty;
    }

    public class BranchRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
    }

    public class ProductRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
    }
}
