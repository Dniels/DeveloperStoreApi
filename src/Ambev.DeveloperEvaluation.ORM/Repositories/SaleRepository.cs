﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DefaultContext _context;

        public SaleRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(x => x.SaleNumber == saleNumber, cancellationToken);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync(int page = 1, int size = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .Include(s => s.Branch)
                .OrderByDescending(x => x.SaleDate)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _context.Sales.AddAsync(sale, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await GetByIdAsync(id, cancellationToken);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Sales.CountAsync(cancellationToken);
        }
    }
}
