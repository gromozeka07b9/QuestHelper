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
                new MainPageMenuItem { Id = 2, Title = "Вокруг меня", TargetType = typeof(AroundMePage), IconName="nearme.png"},
                new MainPageMenuItem { Id = 3, Title = "Профиль", TargetType = typeof(UserProfilePage), IconName="account.png"},
                new MainPageMenuItem { Id = 4, Title = "О нас", TargetType = typeof(AboutPage), IconName="earth.png"},
                new MainPageMenuItem { Id = 5, Title = string.Empty, TargetType = typeof(LoginPage)},

                new MainPageMenuItem { Id = 999, Title = "Main page", TargetType = typeof(MainPage)}//старт view с навигацией по страницам
                });
        }

        public MainPageMenuItem GetPageByPosition(int position)
        {
            return Pages.Single(i=>i.Id == position);
        }

        public MainPageMenuItem GetLoginPage()
        {
            return Pages.Single(x => x.TargetType == typeof(LoginPage));
        }
        public MainPageMenuItem GetMainPage()
        {
            return Pages.Single(x => x.TargetType == typeof(MainPage));
        }
    }
}
