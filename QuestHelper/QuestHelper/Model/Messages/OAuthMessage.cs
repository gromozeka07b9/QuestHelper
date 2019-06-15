using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace QuestHelper.Model.Messages
{
    public class OAuthMessage
    {
        public Uri AuthData;
        public Account OAuthAccount;
    }
}
