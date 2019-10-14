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

namespace QuestHelper.Managers.Sync
{
    public class SyncServer
    {
        public static bool AuthRequired = false;
        private static bool SynchronizeStarted = false;
        Logger _log = new Logger(true);

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
                //syncResult = await syncRoutes.Sync(routeId);
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

        /*public async Task<bool> SyncRoute(string routeId)
        {
            if (!SynchronizeStarted)
            {
                string statusSyncKey = "SyncStatus";
                Analytics.TrackEvent("Sync route");
                SynchronizeStarted = true;
                DateTime startTime = DateTime.Now;
                Application.Current.Properties.Remove(statusSyncKey);
                Application.Current.Properties.Add(statusSyncKey, $"Sync started:{startTime.ToLocalTime().ToString()}");
                _log.AddStringEvent($"Sync route started:{startTime.ToLocalTime().ToString()}");

                var syncResult = await SyncAllRoutes();

                SynchronizeStarted = false;
                var diff = DateTime.Now - startTime;
                string resultMessage = $"Sync route finished at {DateTime.Now.ToLocalTime().ToString()}, due {diff} sec";
                Application.Current.Properties.Remove(statusSyncKey);
                Application.Current.Properties.Add(statusSyncKey, resultMessage);
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = resultMessage }, string.Empty);
            }
        }*/

        public async Task<bool> Sync(string routeId = "")
        {
            bool allSynced = false;
            if (!SynchronizeStarted)
            {
                string statusSyncKey = "SyncStatus";
                Analytics.TrackEvent("Sync all");
                SynchronizeStarted = true;
                DateTime startTime = DateTime.Now;
                Application.Current.Properties.Remove(statusSyncKey);
                Application.Current.Properties.Add(statusSyncKey, $"Sync started:{startTime.ToLocalTime().ToString()}");
                _log.AddStringEvent($"Sync started:{startTime.ToLocalTime().ToString()}");
                Tuple<bool, string> syncResult = new Tuple<bool, string>(false, string.Empty);
                syncResult = string.IsNullOrEmpty(routeId)
                    ? await SyncRoute()
                    : await SyncRoute(routeId);
                SynchronizeStarted = false;
                if (!syncResult.Item1)
                {
                    if (AuthRequired)
                    {
                        var pageCollections = new PagesCollection();
                        MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
                        Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                            new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
                    }
                    var diff = DateTime.Now - startTime;
                    string resultMessage = $"Sync all finished with error:{DateTime.Now.ToLocalTime().ToString()}, due {diff} sec, {syncResult.Item2}";
                    Application.Current.Properties.Remove(statusSyncKey);
                    Application.Current.Properties.Add(statusSyncKey, resultMessage);
                    MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = resultMessage }, string.Empty);
                    Analytics.TrackEvent("Sync error", new Dictionary<string, string> { { "Sync error", syncResult.Item2 } });
                    _log.AddStringEvent(resultMessage);
                }
                else
                {
                    allSynced = true;
                    var diff = DateTime.Now - startTime;
                    string resultMessage = $"Sync finished at {DateTime.Now.ToLocalTime().ToString()}, due {diff} sec";
                    Application.Current.Properties.Remove(statusSyncKey);
                    Application.Current.Properties.Add(statusSyncKey, resultMessage);
                    MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = resultMessage }, string.Empty);

                    double seconds = Math.Round(diff.TotalSeconds);
                    string secondsText = string.Empty;
                    if (seconds < 10) secondsText = "<10s";
                        else if (seconds < 20) secondsText = "<20s";
                            else if (seconds < 50) secondsText = "<50s";
                                else if (seconds < 120) secondsText = "<120s";
                                    else if (seconds >= 120) secondsText = ">=120s";
                    Analytics.TrackEvent("Sync all done", new Dictionary<string, string> { { "Delay", secondsText } });
                    _log.AddStringEvent(resultMessage);
                }
            }
            else
            {
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = "Синхронизация уже запущена" }, string.Empty);
            }

            return allSynced;
        }

        /*private async Task<bool> checkSyncPossibilityAsync(bool showErrorMessageIfExist)
        {
            bool possibility = false;
            bool workInRoaming = false;
            bool isRoaming = DependencyService.Get<INetworkConnectionsService>().IsRoaming();
            if (isRoaming)
            {
                object storedWorkInRoaming;
                if (Application.Current.Properties.TryGetValue("WorkInRoaming", out storedWorkInRoaming))
                {
                    workInRoaming = (bool)storedWorkInRoaming;
                    possibility = workInRoaming;
                }
                else
                {
                    //ToDo:в фоне не работает!
                    workInRoaming = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = "Использовать синхронизацию в роуминге?", Title = "Используется роуминг", OkText = "Да", CancelText = "Нет" });
                    Application.Current.Properties.Add("WorkInRoaming", workInRoaming);
                }
            }
            if (!isRoaming || ((isRoaming) && (workInRoaming)))
            {
                var networkState = Connectivity.NetworkAccess;
                possibility = networkState == NetworkAccess.Internet;
                if ((!possibility) && (showErrorMessageIfExist))
                {
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage(){Title = "Ошибка синхронизации", Message  = "Проверьте ваше подключение к сети"}, string.Empty);
                }
            }

            return possibility;
        }*/

        /*private static async System.Threading.Tasks.Task prepareErrorReportToAppcenterAsync(string errorReport)
        {
            string username = await DependencyService.Get<IUsernameService>().GetUsername();
            Dictionary<string, string> reportLines = new Dictionary<string, string>();
            reportLines.Add("User", username);
            using (StringReader sr = new StringReader(errorReport))
            {
                string line;
                int index = 0;
                while ((line = sr.ReadLine())!=null)
                {

                    reportLines.Add("line" + index, line);
                    index++;
                }
            }
            Crashes.TrackError(new Exception("Error sync media"), reportLines);
        }*/

        /*private static void saveErrorReport(string errorReport)
        {
            try
            {
                string path = Path.Combine(ImagePathManager.GetPicturesDirectory(), "sync.log");
                File.WriteAllText(path, errorReport);
            }
            catch (Exception e)
            {
                HandleError.Process("SyncServer", "saveErrorReport", e, false);
            }
        }*/
    }
}
