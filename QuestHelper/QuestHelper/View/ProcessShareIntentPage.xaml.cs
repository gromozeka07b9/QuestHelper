using QuestHelper.Model.Messages;
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
	public partial class ProcessShareIntentPage : ContentPage
	{
		public ProcessShareIntentPage ()
		{
			InitializeComponent ();	    
        }
	    public ProcessShareIntentPage(ShareFromGoogleMapsMessage msg)
	    {
	        InitializeComponent();
	    }
	}
}