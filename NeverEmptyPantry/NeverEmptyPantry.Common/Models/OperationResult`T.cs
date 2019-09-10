using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models
{
    public class OperationResult<T>
    {
        public IEnumerable<OperationError> Errors { get; set; }
        public bool Succeeded { get; set; }
        public T Data { get; set; }

        public static OperationResult<T> Success => new OperationResult<T> { Succeeded = true };
        public static OperationResult<T> Failed(params OperationError[] errors) =>
             new OperationResult<T>
             {
                 Succeeded = false,
                 Errors = errors
             };
    }
}