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
	public partial class ReceivePushPage : ContentPage
	{
	    private ReceivePushViewModel _vm;
        public ReceivePushPage()
		{
			InitializeComponent ();
		    _vm = new ReceivePushViewModel() { Navigation = this.Navigation };
		    BindingContext = _vm;
		}

	    private void ReceivePushPage_OnAppearing(object sender, EventArgs e)
	    {
            _vm.StartDialog();
	    }

	    private void ReceivePushPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.StopDialog();
	    }
    }
}