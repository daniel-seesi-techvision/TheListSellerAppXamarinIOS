using System;
using CoreGraphics;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.Colors;

namespace TheListSellerAppXamariniOS.Extensions
{
    public static class UIButtonExtensions
    {
       public static UIImageView SetInsideImage(this UIButton button, string image,bool isSystem = false)
       {
            var uiImage = isSystem ? UIImage.GetSystemImage(image) : new UIImage(image);
            var imageView = new UIImageView(uiImage) { TintColor = TransparentButton, ContentMode = UIViewContentMode.ScaleAspectFit };
            button.AddSubview(imageView);
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.CenterXAnchor.ConstraintEqualTo(button.CenterXAnchor).Active = true;
            imageView.CenterYAnchor.ConstraintEqualTo(button.CenterYAnchor).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(button.WidthAnchor,0.5F).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(button.HeightAnchor,0.5F).Active = true;
            return imageView;
        }
    }
}

