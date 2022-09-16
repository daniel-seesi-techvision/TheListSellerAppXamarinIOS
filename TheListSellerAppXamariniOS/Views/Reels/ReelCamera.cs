using System;
using UIKit;
using AVFoundation;
using Xamarin.CommunityToolkit.UI.Views;
using Foundation;
using Xamarin.Forms;
using FFImageLoading.Extensions;
using CoreGraphics;
using TheListSellerAppXamariniOS.Extensions;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelCameraViewController : UIViewController
    {
        private UIButton closeButton, shutterButton, switchCameraButton, flashButton;
        AVCaptureSession cameraSession;
        AVCapturePhotoOutput output;
        AVCaptureVideoPreviewLayer cameraPreveiwLayer;
        UIView cameraRootView;

        public ReelCameraViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupUI();
            checkCameraPermission();
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

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();            
            cameraPreveiwLayer.Frame = cameraRootView.Bounds;
        }

        #region Events

        private void CloseButton_TouchUpInside(object sender, EventArgs e) => DismissViewController(true, null);

        #endregion

        #region Methods
        void SetupUI()
        {
            View.BackgroundColor = UIColor.Black;
            var margins = View.LayoutMarginsGuide;
            
            cameraRootView = new UIView();
            View.AddSubview(cameraRootView);
            cameraRootView.TranslatesAutoresizingMaskIntoConstraints = false;            
            cameraRootView.TopAnchor.ConstraintEqualTo(margins.TopAnchor,10).Active = true;
            cameraRootView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            cameraRootView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
            cameraRootView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -80).Active = true;

            shutterButton = new UIButton() { TintColor = UIColor.White, ContentMode = UIViewContentMode.ScaleAspectFill };
            cameraRootView.AddSubview(shutterButton);
            shutterButton.SetBackgroundImage(new UIImage(SHUTTER),UIControlState.Normal);
            shutterButton.TranslatesAutoresizingMaskIntoConstraints = false;
            shutterButton.Layer.CornerRadius = 50;
            shutterButton.HeightAnchor.ConstraintEqualTo(100).Active = true;
            shutterButton.WidthAnchor.ConstraintEqualTo(100).Active = true;
            shutterButton.BottomAnchor.ConstraintEqualTo(cameraRootView.BottomAnchor, -10).Active = true;
            shutterButton.CenterXAnchor.ConstraintEqualTo(cameraRootView.CenterXAnchor).Active = true;

            switchCameraButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, TintColor = UIColor.White };
            cameraRootView.AddSubview(switchCameraButton);
            switchCameraButton.SetInsideImage(SWITCH_CAMERA);
            switchCameraButton.TranslatesAutoresizingMaskIntoConstraints = false;
            switchCameraButton.Layer.CornerRadius = 25;
            switchCameraButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            switchCameraButton.WidthAnchor.ConstraintEqualTo(shutterButton.WidthAnchor, 0.5F).Active = true;
            switchCameraButton.CenterYAnchor.ConstraintEqualTo(shutterButton.CenterYAnchor).Active = true;
            switchCameraButton.LeadingAnchor.ConstraintEqualTo(shutterButton.TrailingAnchor, 20).Active = true;

            flashButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, TintColor = UIColor.White, ContentMode = UIViewContentMode.Center };
            cameraRootView.AddSubview(flashButton);
            flashButton.SetInsideImage(FLASH);
            flashButton.TranslatesAutoresizingMaskIntoConstraints = false;
            flashButton.Layer.CornerRadius = 25;
            flashButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            flashButton.WidthAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            flashButton.CenterYAnchor.ConstraintEqualTo(shutterButton.CenterYAnchor).Active = true;
            flashButton.TrailingAnchor.ConstraintEqualTo(shutterButton.LeadingAnchor, -20).Active = true;

            closeButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, ContentMode = UIViewContentMode.Center };
            cameraRootView.AddSubview(closeButton);
            closeButton.SetInsideImage(CLOSE);
            closeButton.ContentMode = UIViewContentMode.ScaleToFill;
            closeButton.Layer.CornerRadius = 25;
            closeButton.TranslatesAutoresizingMaskIntoConstraints = false;
            closeButton.TopAnchor.ConstraintEqualTo(cameraRootView.TopAnchor, 20).Active = true;
            closeButton.TrailingAnchor.ConstraintEqualTo(cameraRootView.TrailingAnchor, -20).Active = true;
            closeButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            closeButton.WidthAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;

            var upperInstructionLabel = new UILabel()
            {
                Text = "Upload to your feed",
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };
            cameraRootView.AddSubview(upperInstructionLabel);
            upperInstructionLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            upperInstructionLabel.CenterXAnchor.ConstraintEqualTo(cameraRootView.CenterXAnchor).Active = true;
            upperInstructionLabel.TopAnchor.ConstraintEqualTo(cameraRootView.LayoutMarginsGuide.TopAnchor).Active = true;

            var upperInstructionDescriptionLabel = new UILabel()
            {
                Text = "Share a video or an image to share it with your followers and visitors.",
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                Lines = 3,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };
            cameraRootView.AddSubview(upperInstructionDescriptionLabel);
            upperInstructionDescriptionLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            upperInstructionDescriptionLabel.CenterXAnchor.ConstraintEqualTo(cameraRootView.CenterXAnchor).Active = true;
            upperInstructionDescriptionLabel.WidthAnchor.ConstraintEqualTo(cameraRootView.WidthAnchor, 0.5F).Active = true;
            upperInstructionDescriptionLabel.TopAnchor.ConstraintEqualTo(upperInstructionLabel.BottomAnchor).Active = true;


            var bottomInstructionLabel = new UILabel()
            {
                Text = "Take a picture or tap and hold to take a 30 sec video.",
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.White,
                Lines = 2,
                TextAlignment = UITextAlignment.Right
            };
            View.AddSubview(bottomInstructionLabel);
            bottomInstructionLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomInstructionLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5F).Active = true;
            bottomInstructionLabel.TopAnchor.ConstraintEqualTo(cameraRootView.BottomAnchor).Active = true;
            bottomInstructionLabel.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            bottomInstructionLabel.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor).Active = true;


            var addImageButton = new UIButton() { TintColor = UIColor.White, ContentMode = UIViewContentMode.ScaleAspectFill };
            View.AddSubview(addImageButton);
            addImageButton.SetBackgroundImage(new UIImage(ADD_IMAGE), UIControlState.Normal);
            addImageButton.TranslatesAutoresizingMaskIntoConstraints = false;
            addImageButton.Layer.CornerRadius = 25;
            addImageButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            addImageButton.WidthAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            addImageButton.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            addImageButton.CenterYAnchor.ConstraintEqualTo(bottomInstructionLabel.CenterYAnchor).Active = true;
        }

        void checkCameraPermission()
        {
            switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
            {
                case AVAuthorizationStatus.NotDetermined:
                    // Request
                    AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted =>
                    {
                        if (!granted)
                            return;
                        InvokeOnMainThread(() => SetUpCamera());
                    });
                    break;
                case AVAuthorizationStatus.Authorized:
                    SetUpCamera();
                    break;
                case AVAuthorizationStatus.Restricted:
                    break;
                case AVAuthorizationStatus.Denied:
                    break;

                default:
                    break;
            }
        }

        void SetUpCamera()
        {
            try
            {
                var session = new AVCaptureSession();

                cameraPreveiwLayer = new AVCaptureVideoPreviewLayer(session)
                {
                    Frame = cameraRootView.Layer.Bounds,
                    VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
                    CornerRadius = 10F
                };


                var captureDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaTypes.Video, AVCaptureDevicePosition.Front);
                if (captureDevice == null)
                    // alertService.ShowToast("Sorry, no front camera found");
                    return;

                var input = AVCaptureDeviceInput.FromDevice(captureDevice);
                if (session.CanAddInput(input))
                    session.AddInput(input);

                output = new AVCapturePhotoOutput();
                if (session.CanAddOutput(output))
                    session.AddOutput(output);


                cameraPreveiwLayer.Session = session;
                cameraRootView.Layer.InsertSublayerBelow(cameraPreveiwLayer,shutterButton.Layer);
                session.StartRunning();
                cameraSession = session;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
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

