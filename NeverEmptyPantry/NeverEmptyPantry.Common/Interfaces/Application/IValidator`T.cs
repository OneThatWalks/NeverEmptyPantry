namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IValidator<in T>
    {
        IOperationResult Validate(T obj);
    }
}