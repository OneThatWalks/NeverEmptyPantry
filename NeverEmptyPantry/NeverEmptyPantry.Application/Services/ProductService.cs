using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<IProductService> _logger;

        public ProductService(IRepository<Product> productRepository, ILogger<IProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IOperationResult<Product>> CreateAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IOperationResult<IList<Product>>> ReadAsync(Func<Product, bool> entityQuery)
        {
            if (entityQuery == null)
            {
                throw new ArgumentNullException(nameof(entityQuery));
            }

            IList<Product> repositoryResult;

            try
            {
                repositoryResult = await _productRepository.ReadAsync(entityQuery);
            } catch (Exception e)
            {
                _logger.LogError(e, "An error occured");
                return OperationResult<IList<Product>>.Failed(new OperationError()
                {
                    Name = "Repository",
                    Description = e.Message
                });
            }

            return OperationResult<IList<Product>>.Success(repositoryResult);
        }

        public async Task<IOperationResult<Product>> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IOperationResult<Product>> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}