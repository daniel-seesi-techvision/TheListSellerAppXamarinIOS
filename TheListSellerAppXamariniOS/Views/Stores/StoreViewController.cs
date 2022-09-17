using System;
using System.Collections.Generic;
using CoreGraphics;
using TheListSellerAppXamariniOS.Views.Reels;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.Views.Store
{
    public class StoreViewController : UIViewController
    {
        #region Feilds
        UIButton myFeedButton, myProductButton, videoButton;
        UIView sellerInfoRootView, collectionRootView;

        #endregion
        public StoreViewController() { }

        #region Life cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupUI();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetupReelCollection();
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SubscribeToEvents();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UnSubscribeFromEvents();
        }
        #endregion

        #region Events
        private void VideoButton_TouchUpInside(object sender, EventArgs e)
        {
            var controller = new ReelCameraViewController();
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(controller, true, null);
        }

        private void ButtonTouchUpInsided(object sender, EventArgs e)
        {
            var button = sender as UIButton;
            if (button.Title(UIControlState.Normal) == "MY FEED")
            {
                myFeedButton.SetTitleColor(UIColor.Label, UIControlState.Normal);
                myProductButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
            }
            else
            {
                myProductButton.SetTitleColor(UIColor.Label, UIControlState.Normal);
                myFeedButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
            }

        }
        #endregion

        #region Methods
        void SetupUI()
        {
            #region Nav Control

            var titleLabel = new UILabel()
            {
                Text = MAIN_PAGE_TITLE,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Semibold)
            };

            var gearImage = UIImage.GetSystemImage("gearshape");
            var gearButton = new UIButton()
            {
                TintColor = UIColor.Label
            };
            gearButton.SetBackgroundImage(gearImage,UIControlState.Normal);

            var videoImage = UIImage.GetSystemImage("video.badge.ellipsis");
            videoButton = new UIButton()
            {
                TintColor = UIColor.Label
            };
            videoButton.SetBackgroundImage(videoImage, UIControlState.Normal);

            var trayImage = UIImage.GetSystemImage("tray");
            var trayButton = new UIButton()
            {
                TintColor = UIColor.Label
            };
            trayButton.SetBackgroundImage(trayImage,UIControlState.Normal);
            
            View.AddSubviews(titleLabel,gearButton,videoButton,trayButton);

            View.BackgroundColor = UIColor.SystemBackground;
            var margins = View.LayoutMarginsGuide;

            titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            titleLabel.TopAnchor.ConstraintEqualTo(margins.TopAnchor,10).Active = true;
            titleLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            titleLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor,0.5F).Active = true;
            titleLabel.HeightAnchor.ConstraintEqualTo(30).Active = true;

            gearButton.TranslatesAutoresizingMaskIntoConstraints = false;
            gearButton.TopAnchor.ConstraintEqualTo(margins.TopAnchor,10).Active = true;
            gearButton.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            gearButton.WidthAnchor.ConstraintEqualTo(30).Active = true;
            gearButton.HeightAnchor.ConstraintEqualTo(30).Active = true;

            videoButton.TranslatesAutoresizingMaskIntoConstraints = false;
            videoButton.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            videoButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            videoButton.WidthAnchor.ConstraintEqualTo(30).Active = true;
            videoButton.HeightAnchor.ConstraintEqualTo(30).Active = true;

            trayButton.TranslatesAutoresizingMaskIntoConstraints = false;
            trayButton.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            trayButton.TrailingAnchor.ConstraintEqualTo(videoButton.LeadingAnchor,-20).Active = true;
            trayButton.WidthAnchor.ConstraintEqualTo(30).Active = true;
            trayButton.HeightAnchor.ConstraintEqualTo(30).Active = true;

            #endregion

            #region Seller Info, Feed and Products
            sellerInfoRootView = new UIView();
            sellerInfoRootView.Layer.BorderWidth = 1f;
            sellerInfoRootView.Layer.BorderColor = UIColor.LightGray.CGColor;
            sellerInfoRootView.Alpha = 0.5F;
            View.AddSubview(sellerInfoRootView);
            sellerInfoRootView.TranslatesAutoresizingMaskIntoConstraints = false;
            sellerInfoRootView.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor,10).Active = true;
            sellerInfoRootView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor,-10).Active = true;
            sellerInfoRootView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor,10).Active = true;
            sellerInfoRootView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor,0.35F).Active = true;

            myFeedButton = new UIButton()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold)
            };
            myFeedButton.SetTitle("MY FEED", UIControlState.Normal);
            myFeedButton.SetTitleColor(UIColor.Label, UIControlState.Normal);
            sellerInfoRootView.AddSubview(myFeedButton);
            myFeedButton.TranslatesAutoresizingMaskIntoConstraints = false;
            myFeedButton.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            myFeedButton.BottomAnchor.ConstraintEqualTo(sellerInfoRootView.BottomAnchor).Active = true;
            myFeedButton.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5f).Active = true;
            myFeedButton.HeightAnchor.ConstraintEqualTo(60).Active = true;

            myProductButton = new UIButton()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold)
            };
            myProductButton.SetTitle("MY PRODUCTS", UIControlState.Normal);
            myProductButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
            sellerInfoRootView.AddSubview(myProductButton);
            myProductButton.TranslatesAutoresizingMaskIntoConstraints = false;
            myProductButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            myProductButton.BottomAnchor.ConstraintEqualTo(sellerInfoRootView.BottomAnchor).Active = true;
            myProductButton.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5f).Active = true;
            myProductButton.HeightAnchor.ConstraintEqualTo(60).Active = true;

            collectionRootView = new UIView();
            View.AddSubview(collectionRootView);
            collectionRootView.TranslatesAutoresizingMaskIntoConstraints = false;
            collectionRootView.TopAnchor.ConstraintEqualTo(sellerInfoRootView.BottomAnchor).Active = true;
            collectionRootView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            collectionRootView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
            collectionRootView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            #endregion

            #region Mission Control


            var missionControll = new UIView()
            {
                BackgroundColor = new UIColor(0, 0, 0, 0.5f)
            };
            View.AddSubview(missionControll);
            missionControll.TranslatesAutoresizingMaskIntoConstraints = false;
            missionControll.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor, -20).Active = true;
            missionControll.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20).Active = true;
            missionControll.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20).Active = true;
            missionControll.HeightAnchor.ConstraintEqualTo(70f).Active = true;
            missionControll.Layer.CornerRadius = 35F;
            nfloat buttonSizePercentage = 0.4f;
            nfloat buttonSpacing = 25;

            var home = new UIButton() { TintColor = UIColor.White };
            home.SetBackgroundImage(UIImage.GetSystemImage("homekit"), UIControlState.Normal);

            var hanger = new UIButton() { TintColor = UIColor.White };
            hanger.SetBackgroundImage(new UIImage(HANGER), UIControlState.Normal);

            var shutter = new UIButton() { TintColor = UIColor.White };
            shutter.SetBackgroundImage(new UIImage(SHUTTER), UIControlState.Normal);

            var shippingBox = new UIButton() { TintColor = UIColor.White };
            shippingBox.SetBackgroundImage(UIImage.GetSystemImage("shippingbox"), UIControlState.Normal);

            var requestFeature = new UIButton() { TintColor = UIColor.White };
            requestFeature.SetBackgroundImage(UIImage.GetSystemImage("arrow.2.squarepath"), UIControlState.Normal);

            missionControll.AddSubviews(home,hanger,shutter, requestFeature,shippingBox);

            shutter.TranslatesAutoresizingMaskIntoConstraints = false;
            shutter.HeightAnchor.ConstraintEqualTo(missionControll.HeightAnchor, 0.9f).Active = true;
            shutter.WidthAnchor.ConstraintEqualTo(missionControll.HeightAnchor, 0.9f).Active = true;
            shutter.CenterXAnchor.ConstraintEqualTo(missionControll.CenterXAnchor).Active = true;
            shutter.CenterYAnchor.ConstraintEqualTo(missionControll.CenterYAnchor).Active = true;

            requestFeature.TranslatesAutoresizingMaskIntoConstraints = false;
            requestFeature.HeightAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            requestFeature.WidthAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            requestFeature.LeadingAnchor.ConstraintEqualTo(shutter.TrailingAnchor, buttonSpacing).Active = true;
            requestFeature.CenterYAnchor.ConstraintEqualTo(missionControll.CenterYAnchor).Active = true;

            shippingBox.TranslatesAutoresizingMaskIntoConstraints = false;
            shippingBox.HeightAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            shippingBox.WidthAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            shippingBox.LeadingAnchor.ConstraintEqualTo(requestFeature.TrailingAnchor, buttonSpacing).Active = true;
            shippingBox.CenterYAnchor.ConstraintEqualTo(missionControll.CenterYAnchor).Active = true;

            hanger.TranslatesAutoresizingMaskIntoConstraints = false;
            hanger.HeightAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            hanger.WidthAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            hanger.TrailingAnchor.ConstraintEqualTo(shutter.LeadingAnchor, -buttonSpacing).Active = true;
            hanger.CenterYAnchor.ConstraintEqualTo(missionControll.CenterYAnchor).Active = true;

            home.TranslatesAutoresizingMaskIntoConstraints = false;
            home.HeightAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            home.WidthAnchor.ConstraintEqualTo(missionControll.HeightAnchor, buttonSizePercentage).Active = true;
            home.TrailingAnchor.ConstraintEqualTo(hanger.LeadingAnchor, -buttonSpacing).Active = true;
            home.CenterYAnchor.ConstraintEqualTo(missionControll.CenterYAnchor).Active = true;
            #endregion
        }

        void SetupReelCollection()
        {
            #region Feed Collection
            var collectionLayout = new UICollectionViewFlowLayout();
            collectionLayout.MinimumInteritemSpacing = 1F;
            collectionLayout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            
            //collectionLayout.ItemSize = new CGSize() { Height = collectionRootView.Frame.Height*0.75f, Width = (collectionRootView.Frame.Width / 2) };
            var collectionView = new UICollectionView(collectionRootView.Bounds, collectionLayout);

            // Fetch Reel from database
            var reels = new List<Reel>()
            {
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () },
                //new Reel(){ Description = "", ImageUrl = "", LinkedProducts = new () }
            };

            if (reels.Count is 0)
            {
                var label = new UILabel()
                {
                    Text = "Your Feed is Still Empty Yet.",
                    TextColor = UIColor.Label,
                    Alpha = 0.5F,
                    Font = UIFont.SystemFontOfSize(18,UIFontWeight.Bold),
                    TextAlignment = UITextAlignment.Center
                };
                collectionRootView.AddSubview(label);
                label.TranslatesAutoresizingMaskIntoConstraints = false;
                label.CenterXAnchor.ConstraintEqualTo(collectionRootView.CenterXAnchor).Active = true;
                label.CenterYAnchor.ConstraintEqualTo(collectionRootView.CenterYAnchor,-50F).Active = true;
                label.WidthAnchor.ConstraintEqualTo(collectionRootView.WidthAnchor).Active = true;

                var paragraph = new UILabel()
                {
                    Lines = 3,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = "Upload your content by tapping on \"+\" to promote your products and keep customers updated on new arrivals.",
                    TextColor = UIColor.Label,
                    Alpha = 0.5F,
                    Font = UIFont.SystemFontOfSize(12,UIFontWeight.Thin),
                    TextAlignment = UITextAlignment.Center
                };
                collectionRootView.AddSubview(paragraph);
                paragraph.TranslatesAutoresizingMaskIntoConstraints = false;
                paragraph.CenterXAnchor.ConstraintEqualTo(collectionRootView.CenterXAnchor).Active = true;
                paragraph.TopAnchor.ConstraintEqualTo(label.BottomAnchor,10).Active = true;
                paragraph.WidthAnchor.ConstraintEqualTo(collectionRootView.WidthAnchor, 0.7F).Active = true;
                return;
            }

            collectionView.Delegate = new ReelCollectionLayoutDelegate();
            collectionView.DataSource = new ReelCollectionViewDataSource(reels);
            collectionView.RegisterClassForCell(typeof(ReelCellView), REEL_CELL_ID);
            collectionView.ShowsVerticalScrollIndicator = true;

            collectionRootView.AddSubview(collectionView);
            #endregion
        }

        void SubscribeToEvents()
        {
            videoButton.TouchUpInside += VideoButton_TouchUpInside;
            myFeedButton.TouchUpInside += ButtonTouchUpInsided;
            myProductButton.TouchUpInside += ButtonTouchUpInsided;
        }

        void UnSubscribeFromEvents()
        {

            videoButton.TouchUpInside -= VideoButton_TouchUpInside;
            myFeedButton.TouchUpInside -= ButtonTouchUpInsided;
            myProductButton.TouchUpInside -= ButtonTouchUpInsided;
        }
        #endregion
    }
}