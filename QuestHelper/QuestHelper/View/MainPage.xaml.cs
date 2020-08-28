using Acr.UserDialogs;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using System;
using System.Globalization;
using System.Threading;
using QuestHelper.Resources;
using Xamarin.Forms;
using QuestHelper.WS;
using static QuestHelper.WS.AccountApiRequest;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using System.Linq;

namespace QuestHelper.View
{
    public partial class MainPage : TabbedPage
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
            MessagingCenter.Subscribe<PageNavigationMessage>(this, "", (message)=>
            {
                switch(message.PageToOpen)
                {
                    case MainPages.Private:
                        {
                            CurrentPage = FindByName("PagePrivate") as NavigationPage;
                        }; break;
                    default:
                        {
                            CurrentPage = FindByName("PagePrivate") as NavigationPage;
                        }; break;
                }
            });
        }

    }
}
