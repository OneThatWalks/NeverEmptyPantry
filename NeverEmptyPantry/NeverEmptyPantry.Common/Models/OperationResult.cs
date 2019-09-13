using System.Collections.Generic;
using NeverEmptyPantry.Common.Interfaces;

namespace NeverEmptyPantry.Common.Models
{
    public class OperationResult : IOperationResult
    {
        public IEnumerable<OperationError> Errors { get; set; }
        public bool Succeeded { get; set; }
        public static IOperationResult Success => new OperationResult { Succeeded = true };
        public static IOperationResult Failed(params OperationError[] errors) =>
            new OperationResult
            {
                Succeeded = false,
                Errors = errors
            };
    }
}