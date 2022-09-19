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
        /// <summary>
        /// Add views to parent view and set them up for autolayout
        /// </summary>
        /// <param name="view"></param>
        /// <param name="views"></param>
        public static void AddViews(this UIView view,params UIView[] views)
        {
            view.AddSubviews(views);
            foreach (var child in views)
            {
                child.TranslatesAutoresizingMaskIntoConstraints = false;
            }
        }
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

        public static UIButton CreateCloseButton(this UIView view,UIColor backgroundColor = null)
        {
            var bc = backgroundColor ?? new UIColor(0, 0, 0, 0.5F);
            var closeButton = new UIButton() { BackgroundColor = bc, ContentMode = UIViewContentMode.Center };
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

        public static void RemoveKeyboardOnTap(this UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false; //for iOS5

            view.AddGestureRecognizer(g);
        }
    }
}

