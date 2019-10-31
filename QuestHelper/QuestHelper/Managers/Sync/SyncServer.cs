using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Text;
using QuestHelper.Model.Messages;
using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Essentials;
using System.Threading.Tasks;
using QuestHelper.Model;
using QuestHelper.WS;
using Autofac;

namespace QuestHelper.Managers.Sync
{
    public class SyncServer
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public static bool AuthRequired = false;
        private static bool SynchronizeStarted = false;

        private ITextfileLogger _log;

        public SyncServer()
        {
            _log = App.Container.Resolve<ITextfileLogger>();
        }
        private async System.Threading.Tasks.Task<Tuple<bool, string>> SyncRoute(string routeId = "")
        {
            string errorMsg = string.Empty;
            bool syncResult = false;

            TokenStoreService token = new TokenStoreService();
            string authToken = await token.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(authToken))
            {
                SyncRoutes syncRoutes = new SyncRoutes(authToken);
                syncResult = string.IsNullOrEmpty(routeId)
                    ? await syncRoutes.Sync()
                    : await syncRoutes.Sync(routeId);
                if (syncRoutes.AuthRequired)
                {
                    errorMsg = "Auth required";
                    var pageCollections = new PagesCollection();
                    MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
                    Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                        new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
                }
            }
            return new Tuple<bool, string>(syncResult, errorMsg);
        }

        public async Task<bool> SyncRouteIsNeedAsync(string routeId)
        {
            bool isNeed = false;

            TokenStoreService token = new TokenStoreService();
            string authToken = await token.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(authToken))
            {
                RoutesApiRequest api = new RoutesApiRequest(_apiUrl, authToken);
                var routeServerVersion = await api.GetRouteVersion(routeId);
                if (api.LastHttpStatusCode == HttpStatusCode.OK)
                {
                    RouteManager routeManager = new RouteManager();
                    var routeLocal = routeManager.GetRouteById(routeId);
                    if (routeLocal != null)
                    {
                        isNeed = !routeLocal.ObjVerHash.Equals(routeServerVersion.ObjVerHash);
                    }
                    else isNeed = true;
                }
                else isNeed = true;
            }

            return isNeed;
        }

        /// <summary>
        /// Обновление всех маршрутов, доступных пользователю
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Sync()
        {
            bool allSynced = false;
            if (!SynchronizeStarted)
            {
                string statusSyncKey = "SyncStatus";
                Analytics.TrackEvent("Sync all");
                SynchronizeStarted = true;
                var startTime = prepareProcessSync(statusSyncKey);
                Tuple<bool, string> syncResult = new Tuple<bool, string>(false, string.Empty);
                syncResult = await SyncRoute();
                SynchronizeStarted = false;
                if (!syncResult.Item1)
                {
                    processError(startTime, syncResult, statusSyncKey);
                }
                else
                {
                    allSynced = true;
                    processSuccess(startTime, statusSyncKey);
                }
            }
            else
            {
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = "Синхронизация уже запущена" }, string.Empty);
            }
            return allSynced;
        }

        /// <summary>
        /// Обновление только указанного маршрута
        /// </summary>
        /// <param name="routeId">Id маршрута</param>
        /// <returns></returns>
        public async Task<bool> Sync(string routeId)
        {
            bool allSynced = false;
            string statusSyncKey = "SyncRouteStatus";
            Analytics.TrackEvent("Sync route");
            var startTime = prepareProcessSync(statusSyncKey);
            Tuple<bool, string> syncResult = new Tuple<bool, string>(false, string.Empty);
            syncResult = await SyncRoute(routeId);
            if (!syncResult.Item1)
            {
                processError(startTime, syncResult, statusSyncKey);
            }
            else
            {
                allSynced = true;
                processRouteSuccess(startTime, statusSyncKey, routeId);
            }

            return allSynced;
        }

        private DateTime prepareProcessSync(string statusSyncKey)
        {
            DateTime startTime = DateTime.Now;
            Application.Current.Properties.Remove(statusSyncKey);
            Application.Current.Properties.Add(statusSyncKey, $"Sync started:{startTime.ToLocalTime().ToString()}");
            _log.AddStringEvent($"Sync started:{startTime.ToLocalTime().ToString()}");
            Xamarin.Forms.MessagingCenter.Send<SyncRouteStartMessage>(new SyncRouteStartMessage(), string.Empty);
            return startTime;
        }

        private void processSuccess(DateTime startTime, string statusSyncKey)
        {
            var diff = DateTime.Now - startTime;
            string resultMessage = $"Sync finished at {DateTime.Now.ToLocalTime().ToString()}, due {diff} sec";
            Application.Current.Properties.Remove(statusSyncKey);
            Application.Current.Properties.Add(statusSyncKey, resultMessage);

            double seconds = Math.Round(diff.TotalSeconds);
            string secondsText = string.Empty;
            if (seconds < 10) secondsText = "<10s";
            else if (seconds < 20) secondsText = "<20s";
            else if (seconds < 50) secondsText = "<50s";
            else if (seconds < 120) secondsText = "<120s";
            else if (seconds >= 120) secondsText = ">=120s";
            Analytics.TrackEvent("Sync all done", new Dictionary<string, string> {{"Delay", secondsText}});
            _log.AddStringEvent(resultMessage);
        }
        private void processRouteSuccess(DateTime startTime, string statusSyncKey, string routeId)
        {
            var diff = DateTime.Now - startTime;
            string resultMessage = $"Sync route {routeId} finished at {DateTime.Now.ToLocalTime().ToString()}, due {diff} sec";
            Application.Current.Properties.Remove(statusSyncKey);
            Application.Current.Properties.Add(statusSyncKey, resultMessage);

            //if (string.IsNullOrEmpty(routeId))
            //    MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = resultMessage }, string.Empty);

            double seconds = Math.Round(diff.TotalSeconds);
            string secondsText = string.Empty;
            if (seconds < 10) secondsText = "<10s";
            else if (seconds < 20) secondsText = "<20s";
            else if (seconds < 50) secondsText = "<50s";
            else if (seconds < 120) secondsText = "<120s";
            else if (seconds >= 120) secondsText = ">=120s";
            Analytics.TrackEvent("Sync route done", new Dictionary<string, string> { { "Delay", secondsText } });
            _log.AddStringEvent(resultMessage);
        }

        private void processError(DateTime startTime, Tuple<bool, string> syncResult, string statusSyncKey)
        {
            if (AuthRequired)
            {
                var pageCollections = new PagesCollection();
                MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
                Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                    new PageNavigationMessage()
                    {
                        DestinationPageDescription = destinationPage
                    }, string.Empty);
            }

            var diff = DateTime.Now - startTime;
            string resultMessage =
                $"Sync all finished with error:{DateTime.Now.ToLocalTime().ToString()}, due {diff} sec, {syncResult.Item2}";
            Application.Current.Properties.Remove(statusSyncKey);
            Application.Current.Properties.Add(statusSyncKey, resultMessage);
            MessagingCenter.Send<UIToastMessage>(new UIToastMessage() {Delay = 3, Message = resultMessage}, string.Empty);
            Analytics.TrackEvent("Sync error", new Dictionary<string, string> {{"Sync error", syncResult.Item2}});
            _log.AddStringEvent(resultMessage);
        }
    }
}
