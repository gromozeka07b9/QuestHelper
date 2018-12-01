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
    }
}