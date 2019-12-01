using QuestHelper.Model.Messages;
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
	    public RoutesPage()
	    {
	        InitializeComponent();
	        _vm = new RoutesViewModel() { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }
	    public RoutesPage(ShareFromGoogleMapsMessage msg)
	    {
	        InitializeComponent();
	        _vm = new RoutesViewModel() { Navigation = this.Navigation };
	        _vm.AddSharedPoint(msg);
            BindingContext = _vm;
	    }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            var toolbarService = DependencyService.Get<IToolbarService>();            
            toolbarService.SetVisibilityToolbar(true);
            _vm.startDialog();
            _vm.RefreshListRoutesCommand.Execute(new object());
        }

	    private void RoutesPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.closeDialog();
	    }
	}
}