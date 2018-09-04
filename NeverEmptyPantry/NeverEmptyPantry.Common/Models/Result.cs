using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models
{
    public class Result
    {
        public IEnumerable<Error> Errors { get; set; }
        public bool Succeeded { get; set; }
        public static Result Success => new Result { Succeeded = true };
        public static Result Failed(params Error[] errors) =>
             new Result
             {
                 Succeeded = false,
                 Errors = errors
             };
    }
}