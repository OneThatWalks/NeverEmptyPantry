namespace NeverEmptyPantry.Common.Models.Account
{
    public class LoginResult : Result
    {
        public string Token { get; set; }

        public static LoginResult LoginSuccess(string token) => new LoginResult
        {
            Succeeded = true,
            Token = token
        };

        public static LoginResult LoginFailed(params Error[] errors) => new LoginResult
        {
            Succeeded = false,
            Token = null,
            Errors = errors
        };
    }
}