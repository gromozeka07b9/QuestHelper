﻿using QuestHelper.Model;
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
    public partial class MakeNewRoutePage : ContentPage
    {
        MakeNewRouteViewModel _vm;

        public MakeNewRoutePage(AutoGeneratedRouted autoGeneratedRoute)
        {
            InitializeComponent();
            _vm = new MakeNewRouteViewModel(autoGeneratedRoute);
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

        void SwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
        }

        void SfListView_SwipeEnded(System.Object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            _vm.DeletePointCommand.Execute(e.ItemData);
        }
    }
}