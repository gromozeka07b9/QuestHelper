using QuestHelper.Managers;
using QuestHelper.View;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using QuestHelper.Model;
using QuestHelper.Resources;

namespace QuestHelper.ViewModel
{
    public class RouteCreatedViewModel : INotifyPropertyChanged
    {
        private ViewRoute _vroute;
        private RouteManager _routeManager = new RouteManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenRoutePointDialogCommand { get; private set; }

        public RouteCreatedViewModel(string routeId)
        {
            _vroute = new ViewRoute(routeId);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialog);
        }

        private void openRoutePointDialog()
        {
            Navigation.PushAsync(new RoutePointV2Page(_vroute.Id, string.Empty));
        }

        public void startDialog()
        {
        }

        public string Name
        {
            get
            {
                return CommonResource.RouteCreated_RouteCreatedSuccessful.Replace("[routeName]", _vroute.Name);
            }
        }
    }
}
