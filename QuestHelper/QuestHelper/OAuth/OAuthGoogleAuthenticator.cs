using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using QuestHelper.Model.Messages;
using Xamarin.Auth;

namespace QuestHelper.OAuth
{
    public class OAuthGoogleAuthenticator : IOAuthService
    {
        public bool Login()
        {
            var auth = new OAuth2Authenticator
            (
                clientId: "649860982729-59s7rkt0vrn4d5pf4vt3tmu8tpc9enn4.apps.googleusercontent.com",
                scope: "profile email",
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                redirectUrl: new Uri("com.sd.gosh:/oauth2redirect"),
                clientSecret: null,
                accessTokenUrl: new Uri("https://www.googleapis.com/oauth2/v4/token"),isUsingNativeUI:true
            )
            {
                AllowCancel = true
            };
            auth.Completed += Auth_CompletedAsync;
            auth.Error += Auth_Error;
            AuthenticationState.Authenticator = auth;

            Xamarin.Forms.MessagingCenter.Send<OAuthDialogShowRequest>(new OAuthDialogShowRequest() {}, string.Empty);

            return true;
        }

        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            Analytics.TrackEvent("Login OAuth error", new Dictionary<string, string> { { "ExceptionMessage", e.Exception.Message } });
            Xamarin.Forms.MessagingCenter.Send<OAuthResultMessage>(new OAuthResultMessage() { IsAuthenticated = false }, string.Empty);
        }

        private async void Auth_CompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                AccountStore.Create().Save(e.Account, "com.sd.gosh");
                var request = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v2/userinfo"), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = response.GetResponseText();
                    var user = JsonConvert.DeserializeObject<GoogleUser>(userJson);
                    Xamarin.Forms.MessagingCenter.Send<OAuthResultMessage>(new OAuthResultMessage() { IsAuthenticated = e.IsAuthenticated, Username = user.Name, AuthenticatorUserId = user.Id, Email = user.Email, ImgUrl = user.Picture, Locale = user.Locale, AuthToken = "111"}, string.Empty);
                }
            }
        }

        public void Logout()
        {

        }

        private class GoogleUser
        {
            public string Email = string.Empty;
            public string Name = string.Empty;
            public string Picture = string.Empty;
            public string Locale = string.Empty;
            public string Id = string.Empty;
            /*{
              "id": "111595927184752325457",
              "email": "gromozeka07b9@gmail.com",
              "verified_email": true,
              "name": "Sergey Dyachenko",
              "given_name": "Sergey",
              "family_name": "Dyachenko",
              "picture": "https://lh6.googleusercontent.com/-F5ff-DXURyY/AAAAAAAAAAI/AAAAAAAAAbs/aUuixd7WLTY/photo.jpg",
              "locale": "ru"
            }*/
        }

    }
}