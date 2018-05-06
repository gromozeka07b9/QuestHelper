using QuestHelper.View;
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
