using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; private set; }
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }

        private SaleItem() { } // EF Constructor

        public SaleItem(Guid saleId, Product product, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            SaleId = saleId;
            Product = product ?? throw new ArgumentNullException(nameof(product));
            UnitPrice = unitPrice;
            UpdateQuantity(quantity);
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero");

            if (quantity > 20)
                throw new DomainException("Cannot sell more than 20 identical items");

            Quantity = quantity;
            Discount = CalculateDiscount(quantity, UnitPrice);
            TotalAmount = (UnitPrice * quantity) - Discount;
        }

        public void Cancel()
        {
            IsCancelled = true;
            TotalAmount = 0;
        }

        private static decimal CalculateDiscount(int quantity, decimal unitPrice)
        {
            if (quantity < 4) return 0;

            var discountPercentage = quantity switch
            {
                >= 10 and <= 20 => 0.20m,
                >= 4 => 0.10m,
                _ => 0m
            };

            return unitPrice * quantity * discountPercentage;
        }
    }
}
