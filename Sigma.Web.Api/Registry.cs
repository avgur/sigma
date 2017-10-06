namespace Sigma.Web.Api
{
    public static class Registry
    {
        public static string ApplicationCookie = "cookie";

        public static string EndUserApiPolicy = "EndUserApi";

        public const string OAuthBase64Secret = "_h1BqTyfyQx2RBHDHcbaRCvp3cyaRiZioCoOlkERvCg";
        public const string OAuthAllowedAudience = "sigma.audience";
        public const string OAuthIssuer = "sigma.issuer";
        public const int OAuthAccessTokenExpireSeconds = 300;
        public const int OAuthRefreshTokenExpireSeconds = 86400;
    }
}
