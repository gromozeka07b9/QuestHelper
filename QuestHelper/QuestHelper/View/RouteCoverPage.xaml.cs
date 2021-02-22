using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RouteCoverPage : ContentPage
	{
	    private RouteCoverViewModel _vm;

	    public RouteCoverPage ()
		{
			InitializeComponent ();
    	}

	    public RouteCoverPage(ViewRoute vRoute, bool isNeedShowAlbum)
	    {
	        InitializeComponent();
	        _vm = new RouteCoverViewModel(vRoute, isNeedShowAlbum) { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }

	    public RouteCoverPage(ViewFeedItem vFeedItem)
	    {
	        InitializeComponent();
	        _vm = new RouteCoverViewModel(vFeedItem) { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }

        private void RouteCoverPage_OnAppearing(object sender, EventArgs e)
	    {
            _vm.StartDialog();

	    }

	    private void RouteCoverPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.CloseDialog();
	    }

	}
}