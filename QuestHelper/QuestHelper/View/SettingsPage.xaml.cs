using System;
using System.Collections.Generic;
using QuestHelper.ViewModel;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel _vm;
        public SettingsPage(ref ModalParameters modalParameters)
        {
            InitializeComponent();
            _vm = new SettingsViewModel(ref modalParameters);
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
        }
        
        public SettingsPage()
        {
            InitializeComponent();
        }

        void ContentPage_Appearing(System.Object sender, System.EventArgs e)
        {
            _vm.StartDialog();
        }

        void ContentPage_Disappearing(System.Object sender, System.EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}
