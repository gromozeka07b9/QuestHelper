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
	public partial class RegisterPage : ContentPage
	{
	    LoginViewModel _vm;
	    public RegisterPage()
	    {
	        InitializeComponent();
	        _vm = new LoginViewModel() { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }

	    private void RegisterPage_OnAppearing(object sender, EventArgs e)
	    {
            _vm.StartRegisterDialogAsync();
	    }
	}
}