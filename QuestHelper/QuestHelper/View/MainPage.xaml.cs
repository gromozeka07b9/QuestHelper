﻿using QuestHelper.Model;
using QuestHelper.Model.Messages;
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
            MessagingCenter.Subscribe<PageNavigationMessage>(this, string.Empty, async (sender) => 
            {
                TokenStoreService token = new TokenStoreService();
                if (!string.IsNullOrEmpty(await token.GetAuthTokenAsync()))
                {
                    navigateToPage(sender);
                }
                else
                {
                    var toolbarService = DependencyService.Get<IToolbarService>();
                    if (!toolbarService.ToolbarIsHidden())
                    {
                        toolbarService.SetVisibilityToolbar(false);
                    }
                    var pageCollections = new PagesCollection();
                    navigateToPage(new PageNavigationMessage() { DestinationPageDescription = pageCollections.GetLoginPage() });
                }
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
