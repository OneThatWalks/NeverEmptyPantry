namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IOperationResult<T> : IOperationResult
    {
        T Data { get; set; }
    }
}