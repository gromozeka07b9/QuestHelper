using QuestHelper.ViewModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditRoutePointDescriptionPage : ContentPage
	{
        EditRoutePointDescriptionViewModel vm;
        string _routePointId = string.Empty;


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
            _routePointId = routePointId;
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
			vm.SaveChangedText();
		}

		private void EditorElement_OnFocused(object sender, FocusEventArgs e)
	    {
	        /*var editor = (CustomEditor) sender;
	        if (editor.Text.Equals(editor.Placeholder))
	        {
	            Device.BeginInvokeOnMainThread(() =>
	            {
	                editor.Text = "";
	                editor.Focus();
	            });
	        }*/
        }

	    private void EditorElement_OnUnfocused(object sender, FocusEventArgs e)
	    {
	        /*var editor = (CustomEditor)sender;
	        if (string.IsNullOrEmpty(editor.Text))
	        {
	            Device.BeginInvokeOnMainThread(() =>
	            {
	                editor.Text = editor.Placeholder;
	                editor.Unfocus();
	            });
	        }*/
	    }
    }
}