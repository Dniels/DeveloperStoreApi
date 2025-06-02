using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Unit.Common
{
    public abstract class TestBase : IDisposable
    {
        protected readonly IMapper Mapper;

        protected TestBase()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SaleMappingProfile>();
            });

            Mapper = configuration.CreateMapper();
        }

        public virtual void Dispose()
        {
            // Cleanup resources if needed
        }
    }
}
