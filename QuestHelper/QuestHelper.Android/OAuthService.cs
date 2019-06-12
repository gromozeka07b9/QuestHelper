using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(OAuthService))]
namespace QuestHelper.Droid
{
    public class OAuthService : IOAuthService
    {
        public bool Login()
        {
            var auth = new OAuth2Authenticator
            (
                clientId: "649860982729-59s7rkt0vrn4d5pf4vt3tmu8tpc9enn4.apps.googleusercontent.com",
                scope: "email",
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                redirectUrl: new Uri("com.sd.gosh:/oauth2redirect"),
                clientSecret: null,
                accessTokenUrl: new Uri("https://www.googleapis.com/oauth2/v4/token"),isUsingNativeUI:true
            )
            {
                AllowCancel = true
            };
            auth.Completed += Auth_Completed;
            auth.Error += Auth_Error;
            AuthenticationState.Authenticator = auth;


            var intent = auth.GetUI(Android.App.Application.Context);
            Android.App.Application.Context.StartActivity(intent);
            return true;
        }

        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
        }

        private void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
        }

        public void Logout()
        {

        }
    }
}