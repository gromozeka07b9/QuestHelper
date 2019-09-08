using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareRoutesServicesPage : ContentPage
    {
        private ShareRoutesServicesViewModel _vm;
		public ShareRoutesServicesPage (string routeId)
		{
			InitializeComponent ();
            _vm = new ShareRoutesServicesViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void ShareRoutesServicesPage_OnAppearing(object sender, EventArgs e)
        {
            
        }
    }
}