using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper
{
    public static class HandleError
    {
        public static void Process(string screenName, string actionName, Exception excp, bool showWarning = false)
        {
            Crashes.TrackError(excp, new Dictionary<string, string> { { "Screen", screenName }, { "Action", actionName } });
            if(showWarning)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await App.Current.MainPage.DisplayAlert("Внимание!", excp.Message, "Ок");
                });
            }
        }
    }
}
