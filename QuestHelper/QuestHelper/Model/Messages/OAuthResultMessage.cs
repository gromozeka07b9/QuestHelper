using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace QuestHelper.Model.Messages
{
    /// <summary>
    /// Используется для возврата результата авторизации в окно логина для продолжения работы
    /// </summary>
    public class OAuthResultMessage
    {
        //public Account OAuthAccount;
        public bool IsAuthenticated = false;
        public string Username = string.Empty;
        public string Email = string.Empty;
        public string Locale = string.Empty;
        public string ImgUrl = string.Empty;
        public string AuthenticatorUserId = string.Empty;
        public string AuthToken = string.Empty;
    }
}
