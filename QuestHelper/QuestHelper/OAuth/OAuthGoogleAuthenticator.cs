using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using QuestHelper.Model.Messages;
using System;
using System.Collections.Generic;
using Xamarin.Auth;

namespace QuestHelper.OAuth
{
    public class OAuthGoogleAuthenticator : IOAuthService
    {
        public bool Login()
        {
            var auth = new OAuth2AuthenticatorEx
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
            Analytics.TrackEvent("Login OAuth error", new Dictionary<string, string> { { "ExceptionMessage", e.Message } });
            Xamarin.Forms.MessagingCenter.Send<OAuthResultMessage>(new OAuthResultMessage() { IsAuthenticated = false }, string.Empty);
        }

        private async void Auth_CompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            using (UserDialogs.Instance.Loading("Авторизация...", () => { }, "", true, MaskType.Gradient))
            {
                if (e.IsAuthenticated)
                {
                    Xamarin.Forms.MessagingCenter.Send<SyncProgressRouteLoadingMessage>(new SyncProgressRouteLoadingMessage() { SyncInProgress = true }, string.Empty);
                    //AccountStore.Create().Save(e.Account, "com.sd.gosh");
                    var request = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v2/userinfo"), null, e.Account);
                    var response = await request.GetResponseAsync();
                    if (response != null)
                    {
                        string userJson = response.GetResponseText();
                        var user = JsonConvert.DeserializeObject<GoogleUser>(userJson);
                        Xamarin.Forms.MessagingCenter.Send<OAuthResultMessage>(new OAuthResultMessage() { IsAuthenticated = e.IsAuthenticated, Username = user.Name, AuthenticatorUserId = user.Id, Email = user.Email, ImgUrl = user.Picture, Locale = user.Locale, AuthToken = "111" }, string.Empty);
                    }
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
        }

    }
}