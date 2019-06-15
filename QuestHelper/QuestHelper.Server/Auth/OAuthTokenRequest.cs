using System;
namespace QuestHelper.Server.Auth
{
    public class OAuthTokenRequest
    {
        public string Username = string.Empty;
        public string Email = string.Empty;
        public string Locale = string.Empty;
        public string ImgUrl = string.Empty;
        public string AuthenticatorUserId = string.Empty;
    }
}
