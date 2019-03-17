using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using QuestHelper.Model.Messages;
using Xamarin.Forms;

namespace QuestHelper.Managers.Sync
{
    public class SyncServer
    {
        public static bool AuthRequired = false;

        public static async System.Threading.Tasks.Task<Tuple<bool, string>> SyncAllAsync()
        {
            string errorMsg = string.Empty;
            bool syncResult = false;

            TokenStoreService token = new TokenStoreService();
            string authToken = await token.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(authToken))
            {
                MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage(){SyncInProgress = true, SyncDetailText = "Синхронизация маршрутов..." }, string.Empty);
                SyncRoutes syncRoutes = new SyncRoutes(authToken);
                syncResult = await syncRoutes.Sync();
                AuthRequired = syncRoutes.AuthRequired;
                if (syncResult)
                {
                    MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage() { SyncInProgress = true, SyncDetailText = "Синхронизация точек..." }, string.Empty);
                    SyncPoints syncPoints = new SyncPoints(authToken);
                    syncResult = await syncPoints.Sync();
                    if (syncResult)
                    {
                        MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage() { SyncInProgress = true, SyncDetailText = "Синхронизация медиа..." }, string.Empty);
                        SyncMedia syncMedia = new SyncMedia(authToken);
                        syncResult = await syncMedia.Sync();
                        if (!syncResult)
                        {
                            saveErrorReport(syncMedia.ErrorReport);
                            await prepareErrorReportToAppcenterAsync(syncMedia.ErrorReport);
                            errorMsg = "Ошибка синхронизации файлов!";
                        }
                    }
                    else
                    {
                        errorMsg = "Ошибка синхронизации точек!";
                    }
                }
                else
                {
                    errorMsg = "Ошибка синхронизации маршрутов!";
                }
            }

            MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage() { SyncInProgress = false, SyncDetailText = string.Empty}, string.Empty);
            return new Tuple<bool, string>(syncResult, errorMsg);
    }

        private static async System.Threading.Tasks.Task prepareErrorReportToAppcenterAsync(string errorReport)
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
        }

        private static void saveErrorReport(string errorReport)
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
        }
    }
}
