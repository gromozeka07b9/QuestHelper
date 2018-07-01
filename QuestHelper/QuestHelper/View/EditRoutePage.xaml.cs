using QuestHelper.Model.DB;
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
	public partial class EditRoutePage : ContentPage
	{
		public EditRoutePage()
		{

            InitializeComponent ();
            BindingContext = new EditRouteViewModel(new Route()) { Navigation = this.Navigation };
        }
        public EditRoutePage(Route routeItem)
        {

            InitializeComponent();
            BindingContext = new EditRouteViewModel(routeItem) { Navigation = this.Navigation };
        }
    }
}