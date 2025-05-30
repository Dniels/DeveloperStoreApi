using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public record Branch
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Address { get; init; }

        public Branch(Guid id, string name, string address)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}
