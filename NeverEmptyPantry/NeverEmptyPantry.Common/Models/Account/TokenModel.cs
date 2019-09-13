using System;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class TokenModel
    {
        public string Token { get; set; }

        public TokenModel(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
        }
    }
}