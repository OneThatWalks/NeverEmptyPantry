namespace NeverEmptyPantry.Common.Models.Account
{
    public class RegistrationResult : Result
    {
        public string Token { get; set; }

        public static RegistrationResult RegistrationSuccess(string token) => new RegistrationResult
        {
            Succeeded = true,
            Token = token
        };

        public static RegistrationResult RegistrationFailed(Error[] errors) => new RegistrationResult
        {
            Succeeded = false,
            Token = null,
            Errors = errors
        };
    }
}