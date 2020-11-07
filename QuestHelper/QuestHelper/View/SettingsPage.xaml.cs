using System;
using System.Collections.Generic;
using QuestHelper.ViewModel;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel _vm;
        public SettingsPage()
        {
            InitializeComponent();
            _vm = new SettingsViewModel();
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
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
