namespace TouRest.Application.Common.Constants
{
    public static class AuthConstants
    {
        public const int AccessTokenExpiryMinutes = 15;
        public const int RefreshTokenExpiryDays = 7;

        public const string JwtIssuer = "TouRest";
        public const string JwtAudience = "TouRestClient";
        public const string JwtSecret = "YourSuperSecretKeyForJwtTokenGeneration";

        public const string RefreshTokenCookieName = "refresh_token";
    }
}