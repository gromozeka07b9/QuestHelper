using QuestHelper.LocalDB.Model;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditRoutePointDescriptionPage : ContentPage
	{
        EditRoutePointDescriptionViewModel vm;
        public EditRoutePointDescriptionPage()
		{
            InitializeComponent ();
            vm = new EditRoutePointDescriptionViewModel(string.Empty) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public EditRoutePointDescriptionPage(string routePointId)
        {

            InitializeComponent();
            vm = new EditRoutePointDescriptionViewModel(routePointId) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }

	    private void Editor_OnCompleted(object sender, EventArgs e)
	    {
	        vm.ApplyChanges();
	    }
	}
}