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
    public partial class EditPoiPage : ContentPage
    {
        public EditPoiPage()
        {
            InitializeComponent();
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {

        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {

        }
    }
}