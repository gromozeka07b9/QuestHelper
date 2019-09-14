using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model;

namespace QuestHelper
{
    public interface ICommonShareService
    {
        void Share(ViewRoutePoint vpoint, string packageName);
        /// <summary>
        /// Делимся маршрутом
        /// </summary>
        /// <param name="vroute">Маршрут</param>
        /// <param name="packageName">Название приложения назначения</param>
        void Share(ViewRoute vroute, string packageName);
    }
}
