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
    public partial class SelectTrackFilePage : ContentPage
    {
        private SelectTrackFileViewModel _vm;

        public SelectTrackFilePage(string routeId)
        {
            InitializeComponent();
            _vm = new SelectTrackFileViewModel(routeId) {Navigation = this.Navigation};
            BindingContext = _vm;
            tcs = new System.Threading.Tasks.TaskCompletionSource<OperationResult>();
        }

        private void SelectTrackFilePage_OnAppearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
        }

        private void SelectTrackFilePage_OnDisappearing(object sender, EventArgs e)
        {
            base.OnDisappearing();
            _vm.CloseDialog();
            tcs.SetResult(_vm.DialogResult);
        }

        private TaskCompletionSource<OperationResult> tcs { get; set; }
        public Task<OperationResult> PageClosedTask => tcs.Task;
        
    }
}