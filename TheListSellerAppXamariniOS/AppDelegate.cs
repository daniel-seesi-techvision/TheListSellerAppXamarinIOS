using Foundation;
using TheListSellerAppXamariniOS.DI;
using TheListSellerAppXamariniOS.Services;
using TheListSellerAppXamariniOS.Views;
using TheListSellerAppXamariniOS.Views.Store;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using UIKit;
using TheListSellerAppXamariniOS.Views.Products;
using System.Threading.Tasks;
using System.Collections.Generic;
using TheListSellerAppXamariniOS.Data.Repository;

namespace TheListSellerAppXamariniOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:willFinishLaunchingWithOptions:")]
        public bool WillFinishLaunching(UIApplication application, NSDictionary launchOptions)
        {
            IoC.Init();

            Task.Run(GetProductsAsync, new System.Threading.CancellationToken()).ConfigureAwait(false);

            return true;
        }
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

        async Task GetProductsAsync()
        {
            var requestProvider = IoC.Get<IRequestProvider>();
            var productRepo = IoC.Get<IDataRepository<Product>>();
            await productRepo.TearDownAndRecreateAsync();
            HttpRequestModel requestModel = new HttpRequestModel(PRODUCT_API);
            List<Product> response = await requestProvider.GetAsync<List<Product>>(requestModel);
            await productRepo.CreateMultipleAsync(response);
        }
    }
}


