namespace AspNetCore.JWTDemo.Configurations
{
    public class JwtBearerSettings
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string IssuerSigningKey { get; set; } = string.Empty;
    }
}
