using Foundation;
using TheListSellerAppXamariniOS.Views;
using TheListSellerAppXamariniOS.Views.Store;
using UIKit;

namespace TheListSellerAppXamariniOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var storeView = new StoreViewController();
            Window.RootViewController = storeView;

            // make the window visible
            Window.MakeKeyAndVisible();
            return true;
        }
    }
}


