using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers.Sync
{
    public class SyncServer
    {
        public static async System.Threading.Tasks.Task<Tuple<bool, string>> SyncAllAsync()
        {
            string errorMsg = string.Empty;

            SyncRoutes syncRoutes = new SyncRoutes();
            var syncResult = await syncRoutes.Sync();
            if (syncResult)
            {
                SyncPoints syncPoints = new SyncPoints();
                syncResult = await syncPoints.Sync();
                if (syncResult)
                {
                    SyncMedia syncMedia = new SyncMedia();
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

            return new Tuple<bool, string>(syncResult, errorMsg);
    }
    }
}
