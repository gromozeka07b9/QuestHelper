using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashWizardPage : CarouselPage
	{
	    private SplashWizardViewModel _vm;
        public SplashWizardPage ()
		{
			InitializeComponent ();
		    _vm = new SplashWizardViewModel() { Navigation = this.Navigation };
		    BindingContext = _vm;
		}
    }
}