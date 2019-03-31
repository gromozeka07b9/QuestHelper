using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProcessShareIntentPage = QuestHelper.View.ProcessShareIntentPage;

namespace QuestHelper.Model
{
    public class PagesCollection
    {
        public ObservableCollection<MainPageMenuItem> Pages;

        public PagesCollection()
        {
            Pages = new ObservableCollection<MainPageMenuItem>(new[]
            {
                new MainPageMenuItem { Id = 0, Title = "Маршруты", TargetType = typeof(RoutesPage), IconName="edit.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 1, Title = "Альбомы", TargetType = typeof(AlbumsPage), IconName="share.png", ShowInMenu = true},
                //new MainPageMenuItem { Id = 2, Title = "На карте", TargetType = typeof(MapOverviewPage), IconName="map1.png", ShowInMenu = true},
                //new MainPageMenuItem { Id = 3, Title = "Вокруг меня", TargetType = typeof(AroundMePage), IconName="nearme.png"},
                //new MainPageMenuItem { Id = 4, Title = "Профиль", TargetType = typeof(UserProfilePage), IconName="account.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 5, Title = "О нас", TargetType = typeof(AboutPage), IconName="smile_satisfied.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 6, Title = string.Empty, TargetType = typeof(LoginPage)},
                new MainPageMenuItem { Id = 7, Title = "Обработка выбора", TargetType = typeof(ProcessShareIntentPage), IconName="earth.png"},
                new MainPageMenuItem { Id = 8, Title = "Описание", TargetType = typeof(SplashWizardPage), IconName = "place_unknown.png", ShowInMenu = true},

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
        public MainPageMenuItem GetOverviewMapPage()
        {
            return Pages.Single(x => x.TargetType == typeof(MapOverviewPage));
        }
        public MainPageMenuItem GetSelectRoutesPage()
        {
            return Pages.Single(x => x.TargetType == typeof(RoutesPage));
        }
        public MainPageMenuItem GetProcessSharePage()
        {
            return Pages.Single(x => x.TargetType == typeof(ProcessShareIntentPage));
        }
        public MainPageMenuItem GetProcessWizardPage()
        {
            return Pages.Single(x => x.TargetType == typeof(SplashWizardPage));
        }

        public ObservableCollection<MainPageMenuItem> GetPagesForMenu()
        {
            return new ObservableCollection<MainPageMenuItem>(Pages.Where(x => x.ShowInMenu));
        }
    }
}
