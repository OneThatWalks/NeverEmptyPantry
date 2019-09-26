using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NeverEmptyPantry.Application.Validators;

namespace NeverEmptyPantry.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<IProductService> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IValidatorFactory<Product> _validatorFactory;

        public ProductService(IRepository<Product> productRepository, ILogger<IProductService> logger, IAuthenticationService authenticationService, IValidatorFactory<Product> validatorFactory)
        {
            _productRepository = productRepository;
            _logger = logger;
            _authenticationService = authenticationService;
            _validatorFactory = validatorFactory;
        }

        public async Task<IOperationResult<Product>> CreateAsync(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var validator = _validatorFactory.GetValidator<CreateProductValidator>();
            var validatorResult = validator.Validate(entity);

            if (!validatorResult.Succeeded)
            {
                return OperationResult<Product>.Failed(validatorResult.Errors.ToArray());
            }

            Product repositoryResult;

            var userId = _authenticationService.GetUserId();

            try
            {
                repositoryResult = await _productRepository.CreateAsync(entity, userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured");
                return OperationResult<Product>.Failed(new OperationError()
                {
                    Name = "Repository",
                    Description = e.Message
                });
            }

            return OperationResult<Product>.Success(repositoryResult);
        }

        public async Task<IOperationResult<IList<Product>>> ReadAsync(Expression<Func<Product, bool>> entityQuery)
        {
            if (entityQuery == null)
            {
                throw new ArgumentNullException(nameof(entityQuery));
            }

            IList<Product> repositoryResult;

            try
            {
                repositoryResult = await _productRepository.ReadAsync(entityQuery);
            }
            catch (Exception e)
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
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var validator = _validatorFactory.GetValidator<UpdateProductValidator>();
            var validatorResult = validator.Validate(entity);

            if (!validatorResult.Succeeded)
            {
                return OperationResult<Product>.Failed(validatorResult.Errors.ToArray());
            }

            Product repositoryResult;

            var userId = _authenticationService.GetUserId();

            try
            {
                repositoryResult = await _productRepository.UpdateAsync(entity, userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured");
                return OperationResult<Product>.Failed(new OperationError()
                {
                    Name = "Repository",
                    Description = e.Message
                });
            }

            return OperationResult<Product>.Success(repositoryResult);
        }

        public async Task<IOperationResult<Product>> RemoveAsync(int id)
        {
            Product removeRepositoryResult;
            var userId = _authenticationService.GetUserId();

            try
            {
                var existingProduct = await _productRepository.ReadAsync(id);

                if (existingProduct == null)
                {
                    return OperationResult<Product>.Failed(new OperationError()
                    { Name = "ProductServiceError", Description = "Product does not exist" });
                }

                var validator = _validatorFactory.GetValidator<RemoveProductValidator>();
                var validatorResult = validator.Validate(existingProduct);

                if (!validatorResult.Succeeded)
                {
                    return OperationResult<Product>.Failed(validatorResult.Errors.ToArray());
                }

                removeRepositoryResult = await _productRepository.RemoveAsync(existingProduct, userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured");
                return OperationResult<Product>.Failed(new OperationError()
                {
                    Name = "Repository",
                    Description = e.Message
                });
            }

            return OperationResult<Product>.Success(removeRepositoryResult);
        }
    }
}