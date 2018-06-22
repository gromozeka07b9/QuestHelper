﻿using QuestHelper.ViewModel;
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
	public partial class NewRoutePage : ContentPage
	{
		public NewRoutePage ()
		{

            InitializeComponent ();
            BindingContext = new NewRouteViewModel() { Navigation = this.Navigation };
        }
    }
}