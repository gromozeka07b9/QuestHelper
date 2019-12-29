using QuestHelper.Resources;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FeedRoutesPage = QuestHelper.View.FeedRoutesPage;
using ProcessShareIntentPage = QuestHelper.View.ProcessShareIntentPage;

namespace QuestHelper.Model
{
    public class PagesCollectionDEL
    {
        public ObservableCollection<MainPageMenuItem> Pages;

        public PagesCollectionDEL()
        {
            Pages = new ObservableCollection<MainPageMenuItem>(new[]
            {
                new MainPageMenuItem { Id = 0, Title = CommonResource.ScreenFeedName, TargetType = typeof(FeedRoutesPage), IconName="feed.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 1, Title = CommonResource.ScreenLoadedAlbumsName, TargetType = typeof(AlbumsPage), IconName="loaded.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 2, Title = CommonResource.ScreenPrivateName, TargetType = typeof(RoutesPage), IconName="personal.png", ShowInMenu = true},

                //new MainPageMenuItem { Id = 5, Title = CommonResource.ScreenAboutUsName, TargetType = typeof(AboutPage), IconName="about.png", ShowInMenu = true},
                new MainPageMenuItem { Id = 6, Title = string.Empty, TargetType = typeof(LoginPage)},
                new MainPageMenuItem { Id = 7, Title = CommonResource.ScreenChooseWorkerName, TargetType = typeof(ProcessShareIntentPage), IconName="earth.png"},
                new MainPageMenuItem { Id = 8, Title = CommonResource.ScreenOnBoardingName, TargetType = typeof(SplashWizardPage), IconName = "onboarding.png", ShowInMenu = true},
                //new MainPageMenuItem { Id = 9, Title = "Обновление доступных маршрутов", TargetType = typeof(ReceivePushPage), IconName = "icon.png", ShowInMenu = false},

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
        public MainPageMenuItem GetSelectAlbumsPage()
        {
            return Pages.Single(x => x.TargetType == typeof(AlbumsPage));
        }
        public MainPageMenuItem GetProcessSharePage()
        {
            return Pages.Single(x => x.TargetType == typeof(ProcessShareIntentPage));
        }
        public MainPageMenuItem GetProcessWizardPage()
        {
            return Pages.Single(x => x.TargetType == typeof(SplashWizardPage));
        }
        public MainPageMenuItem GetReceivePushPage()
        {
            return Pages.Single(x => x.TargetType == typeof(ReceivePushPage));
        }
        public MainPageMenuItem GetFeedPage()
        {
            return Pages.Single(x => x.TargetType == typeof(FeedRoutesPage));
        }

        public ObservableCollection<MainPageMenuItem> GetPagesForMenu()
        {
            return new ObservableCollection<MainPageMenuItem>(Pages.Where(x => x.ShowInMenu));
        }
    }
}
