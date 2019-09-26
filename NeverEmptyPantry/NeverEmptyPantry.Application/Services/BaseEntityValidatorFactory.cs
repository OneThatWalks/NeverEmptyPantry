using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Entity;
using System.Collections.Generic;
using System.Linq;

namespace NeverEmptyPantry.Application.Services
{
    public class BaseEntityValidatorFactory<T> : IValidatorFactory<T> where T : IBaseEntity
    {
        private readonly IEnumerable<IValidator<T>> _validators;

        public BaseEntityValidatorFactory(IEnumerable<IValidator<T>> validators)
        {
            _validators = validators;
        }

        public IValidator<T> GetValidator<TValidator>() where TValidator : IValidator<T>
        {
            var validator = _validators.FirstOrDefault(_ => _ is TValidator);

            return validator;
        }
    }
}