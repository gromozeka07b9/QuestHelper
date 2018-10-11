﻿using QuestHelper.LocalDB.Model;
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
    public partial class EditRoutePointDescriptionPage : ContentPage
	{
        EditRoutePointDescriptionViewModel vm;
        public EditRoutePointDescriptionPage()
		{
            InitializeComponent ();
            vm = new EditRoutePointDescriptionViewModel(new RoutePoint()) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public EditRoutePointDescriptionPage(RoutePoint routePoint)
        {

            InitializeComponent();
            vm = new EditRoutePointDescriptionViewModel(routePoint) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}