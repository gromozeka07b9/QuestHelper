using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers.Sync
{
    public class SyncServer
    {
        public static void SyncAll()
        {
            var syncFiles = SyncFiles.GetInstance();
            syncFiles.Start();
            var syncPoints = SyncPoints.GetInstance();
            syncPoints.StartAsync();
        }
    }
}
