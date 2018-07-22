using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            MessagingCenter.Subscribe<PageNavigationMessage>(this, string.Empty, (sender) => {
                navigateToPage(sender);
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
        }

        private void openContentPage(Page page, string title, string iconName)
        {
            page.Title = title;
            page.Icon = new FileImageSource() { File = iconName };

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}
