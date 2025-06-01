using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        private readonly List<SaleItem> _items = new();

        public string SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Customer Customer { get; private set; }
        public Branch Branch { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }
        public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

        private Sale() { } // EF Constructor

        public Sale(string saleNumber, Customer customer, Branch branch)
        {
            Id = Guid.NewGuid();
            SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            Branch = branch ?? throw new ArgumentNullException(nameof(branch));
            SaleDate = DateTime.UtcNow;
            IsCancelled = false;

            AddDomainEvent(new SaleCreatedEvent(Id, SaleNumber, Customer.Id, Branch.Id, SaleDate));
        }

        public void AddItem(Product product, int quantity, decimal unitPrice)
        {
            ValidateQuantity(quantity);
            ValidateNotCancelled();

            var existingItem = _items.FirstOrDefault(x => x.Product.Id == product.Id);
            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                ValidateQuantity(newQuantity);
                existingItem.UpdateQuantity(newQuantity);
            }
            else
            {
                var saleItem = new SaleItem(Id, product, quantity, unitPrice);
                _items.Add(saleItem);
            }

            CalculateTotalAmount();
            AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber));
        }

        public void RemoveItem(Guid productId)
        {
            ValidateNotCancelled();

            var item = _items.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)
            {
                _items.Remove(item);
                CalculateTotalAmount();
                AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber));
            }
        }

        public bool CancelItem(Guid productId)
        {
            ValidateNotCancelled();

            var item = _items.FirstOrDefault(x => x.Id == productId);
            if (item == null)
            {
                item = _items.FirstOrDefault(x => x.Product.Id == productId);
                if (item == null)
                {
                    return false;
                }
            }
            if (item.IsCancelled)
                return true;

            item.Cancel();
            CalculateTotalAmount();
            AddDomainEvent(new ItemCancelledEvent(Id, SaleNumber, productId));
            return true;
        }


        public void Cancel()
        {
            if (IsCancelled) return;

            IsCancelled = true;
            foreach (var item in _items)
            {
                item.Cancel();
            }

            TotalAmount = 0;
            AddDomainEvent(new SaleCancelledEvent(Id, SaleNumber));
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = _items.Where(x => !x.IsCancelled).Sum(x => x.TotalAmount);
        }

        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero");

            if (quantity > 20)
                throw new DomainException("Cannot sell more than 20 identical items");
        }

        private void ValidateNotCancelled()
        {
            if (IsCancelled)
                throw new DomainException("Cannot modify a cancelled sale");
        }

        public void UpdateSaleNumber(string saleNumber)
        {
            ValidateNotCancelled();
            SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
            AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber));
        }

        public void UpdateItems(IEnumerable<(Product product, int quantity, decimal unitPrice)> items)
        {
            ValidateNotCancelled();

            _items.Clear();

            foreach (var (product, quantity, unitPrice) in items)
            {
                AddItem(product, quantity, unitPrice);
            }
        }

        public void UpdateSale(string saleNumber, IEnumerable<(Product product, int quantity, decimal unitPrice)> items)
        {
            UpdateSaleNumber(saleNumber);
            UpdateItems(items);
        }
    }
}
