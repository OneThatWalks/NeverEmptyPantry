namespace NeverEmptyPantry.Common.Models.Identity
{
    public class JwtDetail
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireDays { get; set; }

    }
}