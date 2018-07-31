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
	public partial class RoutesPage : ContentPage
	{
        RoutesViewModel _vm;
        public RoutesPage ()
		{
			InitializeComponent ();
            _vm = new RoutesViewModel() { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.RefreshListRoutesCommand.Execute(new object());
        }
    }
}