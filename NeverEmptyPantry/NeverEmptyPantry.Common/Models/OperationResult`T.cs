using NeverEmptyPantry.Common.Interfaces;
using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models
{
    public class OperationResult<T> : IOperationResult<T>
    {
        public IEnumerable<OperationError> Errors { get; set; }
        public bool Succeeded { get; set; }
        public T Data { get; set; }

        public static IOperationResult<T> Success(T data) => new OperationResult<T> { Succeeded = true, Data = data};
        public static IOperationResult<T> Failed(params OperationError[] errors) =>
             new OperationResult<T>
             {
                 Succeeded = false,
                 Errors = errors
             };
    }
}