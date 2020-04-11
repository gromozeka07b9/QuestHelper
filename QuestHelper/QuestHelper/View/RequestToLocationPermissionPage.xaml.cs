using QuestHelper.Managers;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RequestToLocationPermissionPage : ContentPage
	{
		public RequestToLocationPermissionPage()
		{
			InitializeComponent ();
		}

		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			PermissionManager permissions = new PermissionManager();
			await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
			App.Current.MainPage = new View.MainPage();
		}

		private async void ContentPage_Appearing(object sender, EventArgs e)
		{
			PermissionManager permissions = new PermissionManager();
			bool granted = await permissions.PermissionGetCoordsGrantedAsync();
			if (granted)
			{
				App.Current.MainPage = new View.MainPage();
			}
		}
	}
}