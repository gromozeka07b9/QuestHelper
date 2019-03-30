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
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent ();
		}
	    void OnEmailContactsTapped(object sender, EventArgs args)
	    {
	        try
	        {
	            string email = ((Label)sender).Text;
	            Device.OpenUri(new Uri($"mailto:{email}"));
	        }
	        catch (Exception)
	        {
	        }
	    }
	}
}