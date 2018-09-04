namespace NeverEmptyPantry.Common.Models
{
    public class ErrorCodes
    {
        /* ENTITY FRAMEWORK */
        public const string EntityFrameworkGeneralError = "100";
        public const string EntityFrameworkNotFoundError = "101";
        public const string EntityFrameworkDuplicateWarning = "102";

        /* PARSING AND SERIALIZATION */
        public const string EnumParseError = "200";
    }
}