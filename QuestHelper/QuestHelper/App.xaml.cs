using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace QuestHelper
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            MainPage = new View.MainPage();
            //MainPage = new TabbedMainPage();
            var realm = Realm.GetInstance();
            /*realm.RemoveAll();
            realm.Write(() =>
            {
                realm.Add(new Model.DB.Route() { Name = "test" });
            }
            );*/
        }

        protected override void OnStart ()
		{
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
