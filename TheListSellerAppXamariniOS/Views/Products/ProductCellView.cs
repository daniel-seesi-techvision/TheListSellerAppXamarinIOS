using System;
using TheListSellerAppXamariniOS.Views.Custom;
using UIKit;
using TheListSellerAppXamariniOS.Extensions;
using FFImageLoading;
using static TheListSellerAppXamariniOS.Constants.Colors;

namespace TheListSellerAppXamariniOS.Views.Products
{
    public class ProductCellView : UITableViewCell
    {
        UILabel productLabel;
        UIImageView imageView;
        public CheckBox CheckBox { get; set; }

        public ProductCellView(IntPtr handle):base(handle)
        {
            SetupUI();
        }

        void SetupUI()
        {
            var margins = this.LayoutMarginsGuide;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = TransparentView;

            productLabel = new UILabel()
            {
                TextAlignment = UITextAlignment.Left,
                Lines = 2
            };

            imageView = new UIImageView()
            {
                BackgroundColor = new UIColor(0, 0, 0, 0.03F),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            CheckBox = new CheckBox();

            nfloat height = 100;
            this.AddViews(productLabel, imageView, CheckBox);

            imageView.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(height/2).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(height/2).Active = true;
            imageView.CenterYAnchor.ConstraintEqualTo(this.CenterYAnchor).Active = true;

            CheckBox.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            CheckBox.HeightAnchor.ConstraintEqualTo(height/3).Active = true;
            CheckBox.WidthAnchor.ConstraintEqualTo(height/3).Active = true;
            CheckBox.CenterYAnchor.ConstraintEqualTo(this.CenterYAnchor).Active = true;

            productLabel.CenterYAnchor.ConstraintEqualTo(this.CenterYAnchor).Active = true;
            productLabel.LeadingAnchor.ConstraintEqualTo(imageView.TrailingAnchor,10).Active = true;
            productLabel.TrailingAnchor.ConstraintEqualTo(CheckBox.LeadingAnchor,10).Active = true;
            productLabel.HeightAnchor.ConstraintEqualTo(height).Active = true;

        }

        public void UpdateCell(Product product)
        {
            var formatted = new ManipulatedString(product.Designer + "\n" + product.Title)
                .SetFont(UIFont.SystemFontOfSize(16, UIFontWeight.Semibold), 0, product.Designer.Length)
                .SetFont(UIFont.SystemFontOfSize(12, UIFontWeight.Regular), product.Designer.Length + 1, product.Title.Length);
            productLabel.AttributedText = formatted.FormatedString;

            ImageService.Instance.LoadUrl(product.ImageUrl)
                              .LoadingPlaceholder("placeholder.png")
                              .Retry(3, 2000)
                              .WithCache(FFImageLoading.Cache.CacheType.Disk)
                              .Into(imageView);
            CheckBox.Checked = product.IsSelected;
            CheckBox.UpdateChecked();
        }
    }
}

