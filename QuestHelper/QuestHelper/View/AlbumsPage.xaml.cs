using QuestHelper.ViewModel;
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
	public partial class AlbumsPage : ContentPage
	{
	    AlbumsViewModel _vm;
		public AlbumsPage ()
		{
			InitializeComponent ();
		    _vm = new AlbumsViewModel() { Navigation = this.Navigation };
		    BindingContext = _vm;
		}

	    private void AlbumsPage_OnAppearing(object sender, EventArgs e)
	    {
	        _vm.startDialog();
	    }

        private void AlbumsPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.closeDialog();
	    }
    }
}