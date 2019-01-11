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
		    var editor = this.FindByName<Editor>("EditorElement");
            editor.Keyboard = Keyboard.Create(KeyboardFlags.All);
		}
        public EditRoutePointDescriptionPage(string routePointId)
        {

            InitializeComponent();
            vm = new EditRoutePointDescriptionViewModel(routePointId) { Navigation = this.Navigation };
            BindingContext = vm;
            var editor = this.FindByName<Editor>("EditorElement");
            editor.Keyboard = Keyboard.Create(KeyboardFlags.All);
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