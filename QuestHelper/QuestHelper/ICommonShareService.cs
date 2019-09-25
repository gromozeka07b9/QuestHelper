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

        /// <summary>
        /// Выгрузка только фотографий маршрута
        /// </summary>
        /// <param name="vroute"></param>
        /// <param name="packageName"></param>
        void ShareRouteOnlyPhotos(ViewRoute vroute, string packageName);
        /// <summary>
        /// Выгрузка только описаний маршрута
        /// </summary>
        /// <param name="vroute"></param>
        /// <param name="packageName"></param>
        void ShareRouteOnlyPointsDescription(ViewRoute vroute, string packageName);
    }
}
