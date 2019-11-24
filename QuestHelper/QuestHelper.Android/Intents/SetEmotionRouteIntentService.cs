using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;
using QuestHelper.WS;
using Xamarin.Forms.Platform.Android;

namespace QuestHelper.Droid.Intents
{
    [Service]
    public class SetEmotionRouteIntentService : IntentService
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public SetEmotionRouteIntentService() : base("SetEmotionRouteIntentService")
        {
        }

        protected override async void OnHandleIntent(Intent intent)
        {
            string routeId = intent.GetStringExtra("RouteId") ?? string.Empty;
            bool emotion = intent.GetBooleanExtra("Emotion", true);
            if (!string.IsNullOrEmpty(routeId))
            {
                await SendRequest(routeId, emotion);
            }
        }

        private static async Task SendRequest(string routeId, bool emotion)
        {
            TokenStoreService tokenService = new TokenStoreService();
            string _authToken = await tokenService.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(_authToken))
            {
                var routesApi = new RoutesApiRequest(_apiUrl, _authToken);
                await routesApi.SetEmotionAsync(routeId, emotion);
            }

        }
    }
}