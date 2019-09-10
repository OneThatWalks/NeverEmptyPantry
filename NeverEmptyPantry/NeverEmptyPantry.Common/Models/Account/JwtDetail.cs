namespace NeverEmptyPantry.Common.Models.Account
{
    public class JwtDetail
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireDays { get; set; }

    }
}