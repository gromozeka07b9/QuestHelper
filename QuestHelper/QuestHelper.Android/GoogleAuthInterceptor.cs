using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper;
using QuestHelper.Resources;

namespace QuestHelper.Droid
{
    [Activity(Label = "GoogleAuthInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTask)]
    [
        IntentFilter
        (
            actions: new[] { Intent.ActionView },
            Categories = new[]
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[]
            {
                // First part of the redirect url (Package name)
                "com.sd.gosh"
            },
            DataPaths = new[]
            {
                // Second part of the redirect url (Path)
                "/oauth2redirect"
            }
        )
    ]
    public class GoogleAuthInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            using (UserDialogs.Instance.Loading(CommonResource.Login_AuthorizationProcess, () => { }, "", true, MaskType.Gradient))
            {
                try
                {
                    // Convert Android.Net.Url to Uri
                    var uri = new Uri(Intent.Data.ToString());

                    // Load redirectUrl page
                    AuthenticationState.Authenticator?.OnPageLoading(uri);
                }
                catch (Exception e)
                {
                    HandleError.Process("GoogleAuthInterceptor", "OnCreate load redirect page", e, false);
                }

                //Костыль для обхода ошибки
                //https://github.com/xamarin/Xamarin.Auth/issues/275
                try
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    StartActivity(intent);
                }
                catch (Exception e)
                {
                    HandleError.Process("GoogleAuthInterceptor", "OnCreate StartActivity", e, false);
                }
                //Костыль для обхода ошибки
            }

            Finish();

            return;
        }
    }
}