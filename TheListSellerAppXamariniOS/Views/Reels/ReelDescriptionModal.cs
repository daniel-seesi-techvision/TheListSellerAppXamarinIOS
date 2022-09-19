using System;
using System.ComponentModel;
using System.Linq;
using Foundation;
using TheListSellerAppXamariniOS.Extensions;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.Dimensions;
using static TheListSellerAppXamariniOS.Constants.NotificationConstants;
using static TheListSellerAppXamariniOS.Constants.Colors;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelDescriptionModal : UIViewController, INotifyPropertyChanged, IUITextViewDelegate
    {
        #region Fields
        
        UITextView descriptionTextView;
        UILabel characterCountLabel;
        UIButton closeButton, checkedButton;
        int characterLimit = 140;
        public event PropertyChangedEventHandler PropertyChanged;
        public UIImage reelPhoto;
        #endregion

        public string DescriptionText { get; set; }

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
        }
        #endregion

        #region Events

        private void CloseButton_TouchUpInside(object sender, EventArgs e)
        {
            DescriptionText = descriptionTextView.Text;
            DismissViewController(true, () => NSNotificationCenter.DefaultCenter.PostNotificationName((NSString)DONE_EDITING, this));
        }

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public bool ShouldChangeText(UITextView textView, Foundation.NSRange range, string text)
        {
            string currentText = textView.Text;
            if (string.IsNullOrEmpty(currentText))
                return true;

            string updatedText = "";
            if (text != "")
            {
                // Short circuit execution if current text is already up to 140 to disallow any additions
                if(currentText.Length >= characterLimit)
                    return false;
                // Adding text
                updatedText = currentText.Insert((int)range.Location, text);
            }
            else
            {
                // Removing text
                updatedText = currentText.Remove((int)range.Location, (int)range.Length);
            }

            var charactersLeftLabelText = getCharactersLeftText(updatedText);

            InvokeOnMainThread(() => characterCountLabel.Text = charactersLeftLabelText);
            return updatedText.Length <= characterLimit;
        }


        //TODO find a better way - Perhaps use the DescriptionText Property
        [Export("textViewShouldBeginEditing:")]
        public bool ShouldBeginEditing(UITextView textView)
        {
            if (textView.TextColor == UIColor.LightGray)
            {
                textView.Text = null;
                textView.TextColor = UIColor.White;
            }
            return true;
        }

        //TODO find a better way - Perhaps use the DescriptionText Property
        [Export("textViewShouldEndEditing:")]
        public bool ShouldEndEditing(UITextView textView)
        {
            if (string.IsNullOrEmpty(textView.Text))
            {
                textView.Text = REEL_DESCRIPTION_PLACEHOLDER;
                textView.TextColor = UIColor.LightGray;
            }
            return true;
        }
        #endregion

        #region Methods
        void SetupUI()
        {
            var margins = View.LayoutMarginsGuide;

            View.RemoveKeyboardOnTap();

            var cameraRootView = View.CreateCameraRootView();

            var imageView = new UIImageView(reelPhoto) { ContentMode = UIViewContentMode.ScaleAspectFill, ClipsToBounds = true };
            imageView.Layer.CornerRadius = CAMERA_ROOT_VIEW_CORNER_RADIUS;
            cameraRootView.AddView<UIImageView>(imageView);

            closeButton = cameraRootView.CreateCloseButton();

            var descriptionView = new UIView() { BackgroundColor = new UIColor(0, 0, 0, 0.5F) };
            cameraRootView.AddSubview(descriptionView);
            descriptionView.TranslatesAutoresizingMaskIntoConstraints = false;
            descriptionView.CenterXAnchor.ConstraintEqualTo(margins.CenterXAnchor).Active = true;
            descriptionView.CenterYAnchor.ConstraintEqualTo(margins.CenterYAnchor, -100).Active = true;
            descriptionView.HeightAnchor.ConstraintEqualTo(200).Active = true;
            descriptionView.WidthAnchor.ConstraintEqualTo(margins.WidthAnchor).Active = true;
            descriptionView.Layer.CornerRadius = 5;

            descriptionTextView = new UITextView() { BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.1F), Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular) };
            descriptionTextView.Text = DescriptionText ?? REEL_DESCRIPTION_PLACEHOLDER;
            descriptionTextView.TextColor = string.IsNullOrEmpty(DescriptionText) ? UIColor.LightGray : UIColor.White;
            descriptionView.AddSubview(descriptionTextView);

            descriptionTextView.TranslatesAutoresizingMaskIntoConstraints = false;
            descriptionTextView.LeadingAnchor.ConstraintEqualTo(descriptionView.LeadingAnchor).Active = true;
            descriptionTextView.TrailingAnchor.ConstraintEqualTo(descriptionView.TrailingAnchor).Active = true;
            descriptionTextView.TopAnchor.ConstraintEqualTo(descriptionView.TopAnchor, 5).Active = true;
            descriptionTextView.HeightAnchor.ConstraintEqualTo(descriptionView.HeightAnchor).Active = true;
            descriptionTextView.Delegate = this;

            characterCountLabel = new UILabel() { TextColor = UIColor.White, TextAlignment = UITextAlignment.Right, Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular) };
            characterCountLabel.Text = getCharactersLeftText(DescriptionText ?? "");
            descriptionView.AddSubview(characterCountLabel);
            characterCountLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            characterCountLabel.BottomAnchor.ConstraintEqualTo(descriptionView.BottomAnchor, -10).Active = true;
            characterCountLabel.TrailingAnchor.ConstraintEqualTo(descriptionView.TrailingAnchor, -10).Active = true;
            characterCountLabel.HeightAnchor.ConstraintEqualTo(30).Active = true;
            characterCountLabel.WidthAnchor.ConstraintEqualTo(descriptionView.LayoutMarginsGuide.WidthAnchor).Active = true;

            checkedButton = new UIButton() { BackgroundColor = TransparentButton, TintColor = UIColor.White, ContentMode = UIViewContentMode.Center };
            cameraRootView.AddSubview(checkedButton);
            checkedButton.TranslatesAutoresizingMaskIntoConstraints = false;
            _ = checkedButton.SetInsideImage(CHECKED);
            checkedButton.Layer.CornerRadius = 35;
            checkedButton.HeightAnchor.ConstraintEqualTo(70).Active = true;
            checkedButton.WidthAnchor.ConstraintEqualTo(70).Active = true;
            checkedButton.TopAnchor.ConstraintEqualTo(descriptionView.BottomAnchor, 20).Active = true;
            checkedButton.CenterXAnchor.ConstraintEqualTo(descriptionView.CenterXAnchor).Active = true;
        }

        void SubscribeToEvents()
        {
            closeButton.TouchUpInside += CloseButton_TouchUpInside;
            checkedButton.TouchUpInside += CloseButton_TouchUpInside;
        }

        void UnSubscribeFromEvents()
        {
            closeButton.TouchUpInside -= CloseButton_TouchUpInside;
            checkedButton.TouchUpInside -= CloseButton_TouchUpInside;
        }

        string getCharactersLeftText(string description)
        {
            var left = characterLimit - description.Length;

            return $"{left} Characters left";
        }
        #endregion
    }
}

