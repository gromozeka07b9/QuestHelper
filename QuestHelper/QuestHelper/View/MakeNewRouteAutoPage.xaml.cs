﻿using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.RangeNavigator.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MakeNewRouteAutoPage : ContentPage
    {
        MakeNewRouteAutoViewModel _vm;
        public MakeNewRouteAutoPage()
        {
            InitializeComponent();
            _vm = new MakeNewRouteAutoViewModel();
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }

        private void SfDateTimeRangeNavigator_OnRangeChanged(object sender, RangeChangedEventArgs e)
        {
            _vm.PeriodRouteBegin = e.ViewRangeStartDate;
            _vm.PeriodRouteEnd = e.ViewRangeEndDate.AddDays(1).Date.AddSeconds(-1);
        }
    }
}