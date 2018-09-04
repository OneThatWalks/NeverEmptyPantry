namespace NeverEmptyPantry.Common.Models.Account
{
    public class LogoutResult : Result
    {
        public static LogoutResult LogoutSuccess => new LogoutResult
        {
            Succeeded = true
        };

        public static LogoutResult LogoutFailed(params Error[] errors) => new LogoutResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}