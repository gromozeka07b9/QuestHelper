using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
                SyncRoutes syncRoutes = new SyncRoutes(authToken);
                syncResult = await syncRoutes.Sync();
                AuthRequired = syncRoutes.AuthRequired;
                if (syncResult)
                {
                    SyncPoints syncPoints = new SyncPoints(authToken);
                    syncResult = await syncPoints.Sync();
                    if (syncResult)
                    {
                        SyncMedia syncMedia = new SyncMedia(authToken);
                        syncResult = await syncMedia.Sync();
                        if (!syncResult)
                        {
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


            return new Tuple<bool, string>(syncResult, errorMsg);
    }
    }
}
