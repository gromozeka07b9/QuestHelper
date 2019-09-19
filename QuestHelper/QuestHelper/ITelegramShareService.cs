using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model;

namespace QuestHelper
{
    public interface ITelegramShareService : ICommonShareService
    {
        /// <summary>
        /// Публикуем только фото из маршрута
        /// </summary>
        /// <param name="vroute">Маршрут</param>
        /// <param name="packageName">Название приложения назначения</param>
        void ShareRouteOnlyPhotos(ViewRoute vroute, string packageName);

        /// <summary>
        /// Публикуем только описания из маршрута
        /// </summary>
        /// <param name="vroute">Маршрут</param>
        /// <param name="packageName">Название приложения назначения</param>
        void ShareRouteOnlyPointsDescription(ViewRoute vroute, string packageName);
    }
}
