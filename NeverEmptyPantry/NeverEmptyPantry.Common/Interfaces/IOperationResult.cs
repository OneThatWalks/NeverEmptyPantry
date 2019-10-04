using NeverEmptyPantry.Common.Models;
using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IOperationResult
    {
        IEnumerable<OperationError> Errors { get; set; }
        bool Succeeded { get; set; }
    }
}