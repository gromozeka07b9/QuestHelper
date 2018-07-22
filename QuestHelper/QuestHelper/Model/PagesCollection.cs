using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace QuestHelper.Model
{
    public class PagesCollection
    {
        public ObservableCollection<MainPageMenuItem> Pages;

        public PagesCollection()
        {
            Pages = new ObservableCollection<MainPageMenuItem>(new[]
            {
                new MainPageMenuItem { Id = 0, Title = "Маршруты", TargetType = typeof(RoutesPage), IconName="list.png"},
                new MainPageMenuItem { Id = 1, Title = "Карта", TargetType = typeof(MapOverviewPage), IconName="earth.png"},
                new MainPageMenuItem { Id = 2, Title = "Новый маршрут", TargetType = typeof(RoutePage), IconName="adjust.png"},
                new MainPageMenuItem { Id = 3, Title = "Вокруг меня", TargetType = typeof(AroundMePage), IconName="nearme.png"},
                new MainPageMenuItem { Id = 4, Title = "Профиль", TargetType = typeof(UserProfilePage), IconName="account.png"},
                new MainPageMenuItem { Id = 5, Title = "О нас", TargetType = typeof(AboutPage), IconName="earth.png"}
                });
        }

        public MainPageMenuItem GetPageByPosition(int position)
        {
            return Pages.Single(i=>i.Id == position);
        }
    }
}
