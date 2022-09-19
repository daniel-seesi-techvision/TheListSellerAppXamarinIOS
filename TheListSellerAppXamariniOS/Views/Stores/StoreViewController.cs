using System;
using System.Collections.Generic;
using CoreGraphics;
using TheListSellerAppXamariniOS.Views.Reels;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using static TheListSellerAppXamariniOS.Constants.Dimensions;
using TheListSellerAppXamariniOS.DI;
using TheListSellerAppXamariniOS.Data.Repository;
using System.Linq;

namespace TheListSellerAppXamariniOS.Views.Store
{
    public class StoreViewController : UIViewController
    {
        #region Feilds
        UIButton myFeedButton, myProductButton, videoButton;
        UIView sellerInfoRootView, collectionRootView;
        UILayoutGuide margins;
        private IDataRepository<Reel> reelRepo;

        #endregion
        public StoreViewController() { }

        #region Life cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterServices();
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
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.DarkContent;
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

        // TODO In future use custom UITabView  
        private void ButtonTouchUpInsided(object sender, EventArgs e)
        {
            var button = sender as UIButton;
            if (button.Title(UIControlState.Normal) == "MY FEED")
            {
                myFeedButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                myProductButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                collectionRootView.Hidden = false;
            }
            else
            {
                myProductButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                myFeedButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                collectionRootView.Hidden = true;
            }

        }
        #endregion

        #region Methods
        void RegisterServices()
        {
            reelRepo = IoC.Get<IDataRepository<Reel>>();
        }

        void SetupUI()
        {
            View.BackgroundColor = UIColor.White;
            margins = View.LayoutMarginsGuide;

            #region Nav Control

            var titleLabel = new UILabel()
            {
                Text = MAIN_PAGE_TITLE,
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Semibold)
            };

            videoButton = new UIButton();
            var settingsButton = new UIButton();
            var inboxButton = new UIButton();

            View.AddSubviews(titleLabel, videoButton, settingsButton, inboxButton);


            titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            titleLabel.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            titleLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            titleLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5F).Active = true;
            titleLabel.HeightAnchor.ConstraintEqualTo(30).Active = true;

            SetupNavigationButton(settingsButton, SETTINGS);
            settingsButton.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;

            SetupNavigationButton(videoButton, VIDEO);
            videoButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;

            SetupNavigationButton(inboxButton, INBOX);
            inboxButton.TrailingAnchor.ConstraintEqualTo(videoButton.LeadingAnchor, -20).Active = true;

            #endregion

            #region Seller Info, Feed and Products
            sellerInfoRootView = new UIView();
            sellerInfoRootView.Layer.BorderWidth = 1f;
            sellerInfoRootView.Layer.BorderColor = UIColor.LightGray.CGColor;
            //sellerInfoRootView.Alpha = 0.5F;
            View.AddSubview(sellerInfoRootView);
            sellerInfoRootView.TranslatesAutoresizingMaskIntoConstraints = false;
            sellerInfoRootView.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor, 10).Active = true;
            sellerInfoRootView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, -10).Active = true;
            sellerInfoRootView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, 10).Active = true;
            sellerInfoRootView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.35F).Active = true;

            myFeedButton = new UIButton()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold)
            };
            myFeedButton.SetTitle("MY FEED", UIControlState.Normal);
            myFeedButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
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
            collectionRootView.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor).Active = true;
            #endregion

            #region Mission Control
            var missionControl = new UIView()
            {
                BackgroundColor = UIColor.LightGray
            };
            View.AddSubview(missionControl);
            missionControl.TranslatesAutoresizingMaskIntoConstraints = false;
            missionControl.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor, -20).Active = true;
            missionControl.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20).Active = true;
            missionControl.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20).Active = true;
            missionControl.HeightAnchor.ConstraintEqualTo(70f).Active = true;
            missionControl.Layer.CornerRadius = 35F;
            nfloat buttonSpacing = 25;

            var home = new UIButton();            
            var hanger = new UIButton();
            var package = new UIButton();
            var requestFeature = new UIButton();

            var shutter = new UIButton() { TintColor = UIColor.White };
            shutter.SetBackgroundImage(new UIImage(SHUTTER), UIControlState.Normal);

            missionControl.AddSubviews(home, hanger, shutter, requestFeature, package);

            shutter.TranslatesAutoresizingMaskIntoConstraints = false;
            shutter.HeightAnchor.ConstraintEqualTo(missionControl.HeightAnchor, 0.9f).Active = true;
            shutter.WidthAnchor.ConstraintEqualTo(missionControl.HeightAnchor, 0.9f).Active = true;
            shutter.CenterXAnchor.ConstraintEqualTo(missionControl.CenterXAnchor).Active = true;
            shutter.CenterYAnchor.ConstraintEqualTo(missionControl.CenterYAnchor).Active = true;

            SetupMissionControlButton(home, HOME);
            SetupMissionControlButton(hanger, HANGER);
            SetupMissionControlButton(package, PACKAGE);
            SetupMissionControlButton(requestFeature, REQUEST);

            requestFeature.LeadingAnchor.ConstraintEqualTo(shutter.TrailingAnchor, buttonSpacing).Active = true;
            requestFeature.CenterYAnchor.ConstraintEqualTo(missionControl.CenterYAnchor).Active = true;

            package.LeadingAnchor.ConstraintEqualTo(requestFeature.TrailingAnchor, buttonSpacing).Active = true;
            package.CenterYAnchor.ConstraintEqualTo(missionControl.CenterYAnchor).Active = true;

            hanger.TrailingAnchor.ConstraintEqualTo(shutter.LeadingAnchor, -buttonSpacing).Active = true;
            hanger.CenterYAnchor.ConstraintEqualTo(missionControl.CenterYAnchor).Active = true;

            home.TrailingAnchor.ConstraintEqualTo(hanger.LeadingAnchor, -buttonSpacing).Active = true;
            home.CenterYAnchor.ConstraintEqualTo(missionControl.CenterYAnchor).Active = true;
            #endregion
        }

        void SetupNavigationButton(UIButton button, string image)
        {
            button.TintColor = UIColor.Black;
            button.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            SetupButton(button, image);
        }

        void SetupMissionControlButton(UIButton button, string image)
        {
            button.TintColor = UIColor.White;
            button.HeightAnchor.ConstraintEqualTo(28).Active = true;
            button.WidthAnchor.ConstraintEqualTo(28).Active = true;
            SetupButton(button, image);
        }

        void SetupButton(UIButton button, string image)
        {
            var imageView = new UIImage(image);
            button.SetBackgroundImage(imageView, UIControlState.Normal);
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            button.WidthAnchor.ConstraintEqualTo(30).Active = true;
            button.HeightAnchor.ConstraintEqualTo(30).Active = true;
        }

        void SetupReelCollection()
        {
            var collectionLayout = new UICollectionViewFlowLayout();
            collectionLayout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            collectionLayout.MinimumInteritemSpacing = 1F;
            var collectionView = new UICollectionView(collectionRootView.Bounds, collectionLayout);

            // Fetch Reel from database
            var reels = reelRepo.FindAll().OrderByDescending(m=>m.CreateAt).ToList();

            if (reels.Count is 0)
            {
                var label = new UILabel()
                {
                    Text = "Your Feed is Still Empty Yet.",
                    TextColor = UIColor.LightGray,
                    Font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold),
                    TextAlignment = UITextAlignment.Center
                };
                collectionRootView.AddSubview(label);
                label.TranslatesAutoresizingMaskIntoConstraints = false;
                label.CenterXAnchor.ConstraintEqualTo(collectionRootView.CenterXAnchor).Active = true;
                label.CenterYAnchor.ConstraintEqualTo(collectionRootView.CenterYAnchor, -50F).Active = true;
                label.WidthAnchor.ConstraintEqualTo(collectionRootView.WidthAnchor).Active = true;

                var paragraph = new UILabel()
                {
                    Lines = 3,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = "Upload your content by tapping on \"+\" to promote your products and keep customers updated on new arrivals.",
                    TextColor = UIColor.Label,
                    Alpha = 0.5F,
                    Font = UIFont.SystemFontOfSize(12, UIFontWeight.Thin),
                    TextAlignment = UITextAlignment.Center
                };
                collectionRootView.AddSubview(paragraph);
                paragraph.TranslatesAutoresizingMaskIntoConstraints = false;
                paragraph.CenterXAnchor.ConstraintEqualTo(collectionRootView.CenterXAnchor).Active = true;
                paragraph.TopAnchor.ConstraintEqualTo(label.BottomAnchor, 10).Active = true;
                paragraph.WidthAnchor.ConstraintEqualTo(collectionRootView.WidthAnchor, 0.7F).Active = true;
                return;
            }

            collectionView.Delegate = new ReelCollectionLayoutDelegate();
            collectionView.DataSource = new ReelCollectionViewDataSource(reels);
            collectionView.RegisterClassForCell(typeof(ReelCellView), REEL_CELL_ID);
            collectionView.ShowsVerticalScrollIndicator = true;

            collectionRootView.AddSubview(collectionView);
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