using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(NetworkConnectionsService))]
namespace QuestHelper.Droid
{
    public class NetworkConnectionsService : INetworkConnectionsService
    {
        public bool IsRoaming()
        {
            ConnectivityManager connectivity = ConnectivityManager.FromContext(Application.Context);
            
            if (connectivity?.ActiveNetwork != null)
            {
                var capabilities = connectivity.GetNetworkCapabilities(connectivity.ActiveNetwork);
                if (capabilities != null)
                {
                    return !capabilities.HasCapability(NetCapability.NotRoaming);
                }
            }

            return false;
        }

    }
}