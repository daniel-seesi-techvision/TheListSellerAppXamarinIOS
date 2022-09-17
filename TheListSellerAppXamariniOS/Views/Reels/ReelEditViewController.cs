using System;
using Foundation;
using ObjCRuntime;
using TheListSellerAppXamariniOS.Extensions;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using static TheListSellerAppXamariniOS.Constants.Dimensions;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelEditViewController : UIViewController
    {
        private readonly UIImage reelPhoto;
        UIButton closeButton;

        public ReelEditViewController(NSData data)
        {
            reelPhoto = UIImage.LoadFromData(data);
        }

        #region Life Cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupUI();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SubscribeToEvents();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UnSubscribeFromEvents();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
        }
        #endregion

        #region Events

        private void CloseButton_TouchUpInside(object sender, EventArgs e) => DismissModalViewController(true);
        #endregion

        #region Methods

        void SetupUI()
        {
            View.BackgroundColor = UIColor.Black;

            var cameraRootView = View.CreateCameraRootView();
            
            var imageView = new UIImageView(reelPhoto) { ContentMode = UIViewContentMode.ScaleAspectFill, ClipsToBounds = true };
            imageView.Layer.CornerRadius = CAMERA_ROOT_VIEW_CORNER_RADIUS;
            cameraRootView.AddView<UIImageView>(imageView);

            closeButton = cameraRootView.CreateCloseButton();
        }

        void SubscribeToEvents()
        {
            closeButton.TouchUpInside += CloseButton_TouchUpInside;
        }

        void UnSubscribeFromEvents()
        {
            closeButton.TouchUpInside -= CloseButton_TouchUpInside;
        }
        #endregion
    }
}

