using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.Messages
{
    public class SyncMessage
    {
        public bool ShowErrorMessageIfExist = true;
        /// <summary>
        /// Для синхронизации конкретного маршрута
        /// </summary>
        public string RouteId;

        /// <summary>
        /// Используется при передаче RouteId для того, чтобы указать, что требуется сначала проверить версию
        /// </summary>
        public bool NeedCheckVersionRoute = false;
    }
}
