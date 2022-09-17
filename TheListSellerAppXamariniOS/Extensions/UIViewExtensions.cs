using System;
using System.Drawing.Printing;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using static TheListSellerAppXamariniOS.Constants.Dimensions;

namespace TheListSellerAppXamariniOS.Extensions
{
    public static class UIViewExtensions
    {
        public static UIView CreateCameraRootView(this UIView view)
        {
            var margins = view.LayoutMarginsGuide;
            var cameraRootView = new UIView();
            view.AddSubview(cameraRootView);
            cameraRootView.TranslatesAutoresizingMaskIntoConstraints = false;
            cameraRootView.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            cameraRootView.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor).Active = true;
            cameraRootView.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
            cameraRootView.BottomAnchor.ConstraintEqualTo(view.BottomAnchor, CAMERA_ROOT_VIEW_BOTTOM_ANCHOR).Active = true;
            return cameraRootView;
        }

        public static UIButton CreateCloseButton(this UIView view)
        {
            var closeButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, ContentMode = UIViewContentMode.Center };
            view.AddSubview(closeButton);
            _ = closeButton.SetInsideImage(CLOSE);
            closeButton.ContentMode = UIViewContentMode.ScaleToFill;
            closeButton.Layer.CornerRadius = 25;
            closeButton.TranslatesAutoresizingMaskIntoConstraints = false;
            closeButton.TopAnchor.ConstraintEqualTo(view.TopAnchor, 20).Active = true;
            closeButton.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor, -20).Active = true;
            closeButton.HeightAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            closeButton.WidthAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            return closeButton;
        }

        /// <summary>
        /// Add the inner view with constraints to fill container view
        /// </summary>
        /// <param name="containerView">container view</param>
        /// <returns></returns>
        public static void AddView<T>(this UIView containerView,T innerView) where T : UIView
        {
            containerView.AddSubview(innerView);
            innerView.TranslatesAutoresizingMaskIntoConstraints = false;
            innerView.TopAnchor.ConstraintEqualTo(containerView.TopAnchor).Active = true;
            innerView.LeadingAnchor.ConstraintEqualTo(containerView.LeadingAnchor).Active = true;
            innerView.TrailingAnchor.ConstraintEqualTo(containerView.TrailingAnchor).Active = true;
            innerView.BottomAnchor.ConstraintEqualTo(containerView.BottomAnchor).Active = true;
        }
    }
}

