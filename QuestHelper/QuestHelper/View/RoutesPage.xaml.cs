using QuestHelper.Model.Messages;
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
	public partial class RoutesPage : ContentPage
	{
        RoutesViewModel _vm;
	    public RoutesPage()
	    {
	        InitializeComponent();
	        _vm = new RoutesViewModel() { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }
	    public RoutesPage(ShareFromGoogleMapsMessage msg)
	    {
	        InitializeComponent();
	        _vm = new RoutesViewModel() { Navigation = this.Navigation };
	        _vm.AddSharedPoint(msg);
            BindingContext = _vm;
	    }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.startDialog();
            _vm.RefreshListRoutesCommand.Execute(new object());
			SyncAnimation.IsVisible = true;
			if (_vm.IsVisibleProgress)
			{
				SyncAnimation.Play();//Какой-то глюк есть - когда анимация уходит в невидимую область списка, при возврате она уже не работает, приходится повторно пинать
			}
			else
			{
				SyncAnimation.Pause();
			}

		}

		public void StopAnimateCallback()
		{
			SyncAnimation.Pause();
		}

		private void RoutesPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.closeDialog();
			MessagingCenter.Unsubscribe<SyncRouteCompleteMessage>(this, string.Empty);
		}

		private void AnimationView_OnClick(object sender, EventArgs e)
		{
			SyncAnimation.Play();//Какой-то глюк есть - когда анимация уходит в невидимую область списка, при возврате она уже не работает, приходится повторно пинать
			if (!_vm.IsVisibleProgress)
			{
				SyncAnimation.Play();
				Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);//Запуск новой синхронизации
			}
		}
	}
}