namespace NeverEmptyPantry.Api.Models
{
    public class TokenResult
    {
        public TokenResult(string token)
        {
            Token = token;
        }
        public string Token { get; set; }
    }
}