using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuestHelper.Droid
{
    public class AuthService : IAuthService
    {
        public string GetAuthToken()
        {
            return "this token";
        }
    }
}