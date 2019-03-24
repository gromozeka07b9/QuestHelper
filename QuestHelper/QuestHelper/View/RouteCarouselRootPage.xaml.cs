using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteCarouselRootPage : ContentPage
    {
        private PointCarouselRootViewModel _vm;
        public RouteCarouselRootPage()
        {
            InitializeComponent();
            _vm = new PointCarouselRootViewModel("","","") { Navigation = this.Navigation };
            BindingContext = _vm;

            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            var list = manager.GetAllMediaObjects();
            List<Image> imgs = new List<Image>();
            foreach (var item in list)
            {
                Image img = new Image(){Source = ImagePathManager.GetImagePath(item.RoutePointMediaObjectId, true)};
                imgs.Add(img);
            }
            Cards.ItemsSource = imgs;
        }
    }
}