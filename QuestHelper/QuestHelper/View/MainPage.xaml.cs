using Acr.UserDialogs;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using System;
using System.Globalization;
using System.Threading;
using QuestHelper.Resources;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
#if DEBUG
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
#endif

            var pageCollections = new PagesCollection();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MessagingCenter.Subscribe<PageNavigationMessage>(this, string.Empty, async (senderMsg) =>
            {
                if (senderMsg.DestinationPageDescription.TargetType == typeof(SplashWizardPage))
                {
                    await Navigation.PushModalAsync(new NavigationPage(new SplashWizardPage()));
                }
                else
                {
                    TokenStoreService token = new TokenStoreService();
                    if (!string.IsNullOrEmpty(await token.GetAuthTokenAsync()))
                    {
                        navigateToPage(senderMsg);
                    }
                    else
                    {
                        navigateToPage(new PageNavigationMessage() { DestinationPageDescription = pageCollections.GetLoginPage() });
                    }
                }
            });
            MessagingCenter.Subscribe<ShareFromGoogleMapsMessage>(this, string.Empty, (senderMsg) =>
            {
                UserDialogs.Instance.Alert(CommonResource.ShareMsg_ChooseRouteForAddPoint, CommonResource.ShareMsg_MakingNewPoint);
                MessagingCenter.Unsubscribe<ShareFromGoogleMapsMessage>(this, string.Empty);
                var pageParameters = pageCollections.GetSelectRoutesPage();
                var page = (RoutesPage)Activator.CreateInstance(pageParameters.TargetType, args: senderMsg);
                openContentPage(page, CommonResource.ShareMsg_ChooseWorker, "");
            });
        }

        private void navigateToPage(PageNavigationMessage msg)
        {
            if(msg.DestinationPageDescription!=null)
            {
                var page = (Page)Activator.CreateInstance(msg.DestinationPageDescription.TargetType);
                openContentPage(page, msg.DestinationPageDescription.Title, msg.DestinationPageDescription.IconName);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MainPageMenuItem item = e.SelectedItem as MainPageMenuItem;
            if (item != null)
                MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = item }, string.Empty);
            MasterPage.ListView.SelectedItem = null;
        }

        private void openContentPage(Page page, string title, string iconName)
        {
            page.Title = title;
            //page.Icon = new FileImageSource() { File = iconName };
            page.IconImageSource = new FileImageSource() { File = iconName };
            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private async void MainPage_OnAppearing(object sender, EventArgs e)
        {
            var pageCollections = new PagesCollection();
            MessagingCenter.Subscribe<ToggleFullscreenMapMessage>(this, string.Empty, (senderMsg) =>
            {
                var pageParameters = pageCollections.GetOverviewMapPage();
                var page = (MapOverviewPage)Activator.CreateInstance(pageParameters.TargetType);
                //page.CurrentRouteId = sender.RouteId;
                openContentPage(page, pageParameters.Title, pageParameters.IconName);
            });

            ParameterManager par = new ParameterManager();
            string showOnboarding = string.Empty;
            if (par.Get("NeedShowOnboarding", out showOnboarding))
            {
                if (showOnboarding == "1")
                {
                    DependencyService.Get<IToolbarService>().SetVisibilityToolbar(false);
                    await Navigation.PushModalAsync(new NavigationPage(new SplashWizardPage()));
                }
            }
        }

        private void MainPage_OnDisappearing(object sender, EventArgs e)
        {
            MessagingCenter.Unsubscribe<ShareFromGoogleMapsMessage>(this, string.Empty);
        }
    }
}
