using System.Collections.Generic;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IOperationResult
    {
        IEnumerable<OperationError> Errors { get; set; }
        bool Succeeded { get; set; }
    }
}