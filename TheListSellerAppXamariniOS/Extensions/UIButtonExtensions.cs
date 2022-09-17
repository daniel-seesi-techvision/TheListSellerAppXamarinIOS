using System;
using CoreGraphics;
using UIKit;

namespace TheListSellerAppXamariniOS.Extensions
{
    public static class UIButtonExtensions
    {
       public static UIImageView SetInsideImage(this UIButton button, string image)
       {            
            var switchImageView = new UIImageView(new UIImage(image)) { TintColor = UIColor.White,ContentMode = UIViewContentMode.ScaleAspectFit };
            button.AddSubview(switchImageView);
            switchImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            switchImageView.CenterXAnchor.ConstraintEqualTo(button.CenterXAnchor).Active = true;
            switchImageView.CenterYAnchor.ConstraintEqualTo(button.CenterYAnchor).Active = true;
            switchImageView.WidthAnchor.ConstraintEqualTo(button.WidthAnchor,0.5F).Active = true;
            switchImageView.HeightAnchor.ConstraintEqualTo(button.HeightAnchor,0.5F).Active = true;
            return switchImageView;
        }
    }
}

