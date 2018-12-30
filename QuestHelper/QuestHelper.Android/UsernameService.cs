using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using QuestHelper.Droid;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.Dependency(typeof(UsernameService))]
namespace QuestHelper.Droid
{
    public class UsernameService : IUsernameService
    {
        public async Task<string> GetUsername()
        {
            FormsAppCompatActivity activity = CrossCurrentActivity.Current.Activity as FormsAppCompatActivity;
            AccountManager manager = AccountManager.Get(activity);
            Account[] accounts = manager.GetAccountsByType("com.google");
            if (accounts.Length > 0)
            {
                return accounts.First().Name;
            }
            return "";
        }
    }
}