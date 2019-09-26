namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IValidatorFactory<TEntity>
    {
        IValidator<TEntity> GetValidator<TValidator>() where TValidator : IValidator<TEntity>;
    }
}