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
	public partial class LoginPage : ContentPage
	{
	    LoginViewModel _vm;
		public LoginPage ()
		{
			InitializeComponent ();
		    _vm = new LoginViewModel() { Navigation = this.Navigation };
		    BindingContext = _vm;
		}

	    private void LoginPage_OnAppearing(object sender, EventArgs e)
	    {
	        var toolbarService = DependencyService.Get<IToolbarService>();
	        toolbarService.SetVisibilityToolbar(false);
	    }

        private void LoginPage_OnDisappearing(object sender, EventArgs e)
	    {
	        var toolbarService = DependencyService.Get<IToolbarService>();
	        toolbarService.SetVisibilityToolbar(true);
	    }
    }
}