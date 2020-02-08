using Foundation;
using QuestHelper.iOS;
using System;
using System.Linq;
using UIKit;
using Path = System.IO.Path;

[assembly: Xamarin.Forms.Dependency(typeof(DefaultViewer))]
namespace QuestHelper.iOS
{
    public class DefaultViewer : IDefaultViewer
    {
        public void Show(string filename)
        {
            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    var firstController = UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First();
                    var navcontroller = firstController as UINavigationController;
                    var viewController = navcontroller.ViewControllers.Last();
                    var uidic = UIDocumentInteractionController.FromUrl(new NSUrl(filename, true));

                    uidic.Delegate = new DocInteractionC(viewController);

                    uidic.PresentPreview(true);
                });
            }
            catch (Exception e)
            {

            }

        }
        public class DocInteractionC : UIDocumentInteractionControllerDelegate
        {
            readonly UIViewController _viewController;

            public DocInteractionC(UIViewController controller)
            {
                _viewController = controller;
            }

            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
            {

                return _viewController;

            }

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
            {

                return _viewController.View;

            }

        }
    }
}