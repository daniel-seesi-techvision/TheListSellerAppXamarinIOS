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
using static TheListSellerAppXamariniOS.Constants.Dimensions;
using CoreFoundation;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelCameraViewController : UIViewController, IAVCapturePhotoCaptureDelegate, INotifyPropertyChanged
    {
        #region Fields

        UIButton closeButton, shutterButton, switchCameraButton, flashButton;
        UIImageView flashImageView;
        AVCaptureSession cameraSession;
        AVCaptureDeviceInput captureInput;
        AVCapturePhotoOutput captureOutput;
        AVCaptureVideoPreviewLayer cameraPreveiwLayer;
        readonly AVCaptureDeviceDiscoverySession videoDeviceDiscoverySession = AVCaptureDeviceDiscoverySession.Create(
            new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDualCamera },
            AVMediaType.Video,
            AVCaptureDevicePosition.Unspecified);
        AVCaptureFlashMode _currentFlashMode;
        UIView cameraRootView;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        AVCaptureFlashMode CurrentFlashMode
        {
            get => _currentFlashMode;
            set
            {
                if (_currentFlashMode == value)
                    return;

                _currentFlashMode = value;
                OnPropertyChanged();
            }
        }

        public ReelCameraViewController()
        {
        }

        #region Life Cycle

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
        #endregion

        #region Events

        private void CloseButton_TouchUpInside(object sender, EventArgs e) => DismissViewController(true, null);

        private void ShutterButton_TouchUpInside(object sender, EventArgs e)
        {
            var captureSettings = AVCapturePhotoSettings.Create();
            captureSettings.FlashMode = CurrentFlashMode;
            captureOutput.CapturePhoto(captureSettings, this);
        }

        private void SwitchCameraButton_TouchUpInside(object sender, EventArgs e) => SwitchCamera();

        private void FlashButton_TouchUpInside(object sender, EventArgs e) => ToggleFlashMode();

        private void OnPropertyChangedHere(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentFlashMode))
            {
                InvokeOnMainThread(() => UpdateFlashModeUI());
            }

        }
        #endregion

        #region Methods
        void SetupUI()
        {
            View.BackgroundColor = UIColor.Black;

            var margins = View.LayoutMarginsGuide;

            cameraRootView = View.CreateCameraRootView();

            shutterButton = new UIButton() { TintColor = UIColor.White, ContentMode = UIViewContentMode.ScaleAspectFill };
            cameraRootView.AddSubview(shutterButton);
            shutterButton.SetBackgroundImage(new UIImage(SHUTTER), UIControlState.Normal);
            shutterButton.TranslatesAutoresizingMaskIntoConstraints = false;
            shutterButton.Layer.CornerRadius = 50;
            shutterButton.HeightAnchor.ConstraintEqualTo(100).Active = true;
            shutterButton.WidthAnchor.ConstraintEqualTo(100).Active = true;
            shutterButton.BottomAnchor.ConstraintEqualTo(cameraRootView.BottomAnchor, -10).Active = true;
            shutterButton.CenterXAnchor.ConstraintEqualTo(cameraRootView.CenterXAnchor).Active = true;

            switchCameraButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, TintColor = UIColor.White };
            cameraRootView.AddSubview(switchCameraButton);
            _ = switchCameraButton.SetInsideImage(SWITCH_CAMERA);
            switchCameraButton.TranslatesAutoresizingMaskIntoConstraints = false;
            switchCameraButton.Layer.CornerRadius = 25;
            switchCameraButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            switchCameraButton.WidthAnchor.ConstraintEqualTo(shutterButton.WidthAnchor, 0.5F).Active = true;
            switchCameraButton.CenterYAnchor.ConstraintEqualTo(shutterButton.CenterYAnchor).Active = true;
            switchCameraButton.LeadingAnchor.ConstraintEqualTo(shutterButton.TrailingAnchor, 20).Active = true;

            flashButton = new UIButton() { BackgroundColor = UIColor.Black, Alpha = 0.5F, TintColor = UIColor.White, ContentMode = UIViewContentMode.Center };
            cameraRootView.AddSubview(flashButton);
            CurrentFlashMode = AVCaptureFlashMode.On;
            flashImageView = flashButton.SetInsideImage(FLASH);
            flashButton.TranslatesAutoresizingMaskIntoConstraints = false;
            flashButton.Layer.CornerRadius = 25;
            flashButton.HeightAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            flashButton.WidthAnchor.ConstraintEqualTo(shutterButton.HeightAnchor, 0.5F).Active = true;
            flashButton.CenterYAnchor.ConstraintEqualTo(shutterButton.CenterYAnchor).Active = true;
            flashButton.TrailingAnchor.ConstraintEqualTo(shutterButton.LeadingAnchor, -20).Active = true;

            closeButton = cameraRootView.CreateCloseButton();

            #region Views below Camera Root View

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
            #endregion

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
                        InvokeOnMainThread(() => InitCamera(AVCaptureDevicePosition.Back));
                    });
                    break;
                case AVAuthorizationStatus.Authorized:
                    InitCamera(AVCaptureDevicePosition.Back);
                    break;
                case AVAuthorizationStatus.Restricted:
                    break;
                case AVAuthorizationStatus.Denied:
                    break;

                default:
                    break;
            }
        }

        void InitCamera(AVCaptureDevicePosition preferredPosition)
        {
            try
            {
                AVCaptureDeviceType preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;

                switch (preferredPosition)
                {
                    case AVCaptureDevicePosition.Unspecified:
                        Debug.WriteLine("Camera Position is Unspecified", "Switch Camera");
                        break;
                    case AVCaptureDevicePosition.Front:
                        preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
                        break;
                    case AVCaptureDevicePosition.Back:
                        preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;
                        break;
                }

                SetCameraSession(preferredPosition, preferredDeviceType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void SwitchCamera()
        {
            try
            {
                var currentVideoDevice = captureInput.Device;
                var currentPosition = currentVideoDevice.Position;
                AVCaptureDevicePosition preferredPosition = AVCaptureDevicePosition.Unspecified;
                AVCaptureDeviceType preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
                switch (currentPosition)
                {
                    case AVCaptureDevicePosition.Unspecified:
                        Debug.WriteLine("Camera Position is Unspecified", "Switch Camera");
                        break;
                    case AVCaptureDevicePosition.Front:
                        preferredPosition = AVCaptureDevicePosition.Back;
                        preferredDeviceType = AVCaptureDeviceType.BuiltInDualCamera;
                        break;
                    case AVCaptureDevicePosition.Back:
                        preferredPosition = AVCaptureDevicePosition.Front;
                        preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
                        break;
                }

                SetCameraSession(preferredPosition, preferredDeviceType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void SetCameraSession(AVCaptureDevicePosition preferredPosition, AVCaptureDeviceType preferredDeviceType)
        {
            var devices = videoDeviceDiscoverySession.Devices;
            AVCaptureDevice newDevice = null;

            // First, look for a device with both the preferred position and device type.
            foreach (var device in devices)
            {
                if (device.Position == preferredPosition && device.DeviceType.GetConstant() == preferredDeviceType.GetConstant())
                {
                    newDevice = device;
                    break;
                }
            }

            // Otherwise, look for a device with only the preferred position.
            if (newDevice == null)
            {
                foreach (var device in devices)
                {
                    if (device.Position == preferredPosition)
                    {
                        newDevice = device;
                        break;
                    }
                }
            }

            if (newDevice == null)
                return;

            //var captureDevice = AVCaptureDevice.GetDefaultDevice(preferredDeviceType, AVMediaTypes.Video, preferredPosition);
            //if (captureDevice == null)
            //    return;
            var session = new AVCaptureSession();

            bool layerIsAlreadyInView = cameraRootView.Layer.Sublayers.Any(mn => mn == cameraPreveiwLayer);
            if (layerIsAlreadyInView)
                cameraPreveiwLayer.RemoveFromSuperLayer();

            cameraPreveiwLayer = new AVCaptureVideoPreviewLayer(session)
            {
                Frame = cameraRootView.Layer.Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
                CornerRadius = CAMERA_ROOT_VIEW_CORNER_RADIUS
            };


            cameraRootView.Layer.InsertSublayerBelow(cameraPreveiwLayer, shutterButton.Layer);

            if (captureInput is not null)
                session.RemoveInput(captureInput);

            captureInput = AVCaptureDeviceInput.FromDevice(newDevice);
            if (session.CanAddInput(captureInput))
                session.AddInput(captureInput);

            if (captureOutput is not null)
                session.RemoveOutput(captureOutput);

            captureOutput = new AVCapturePhotoOutput();
            if (session.CanAddOutput(captureOutput))
                session.AddOutput(captureOutput);


            cameraPreveiwLayer.Session = session;
            cameraPreveiwLayer.Connection.AutomaticallyAdjustsVideoMirroring = false;
            cameraPreveiwLayer.Mirrored = false;

            session.StartRunning();
            cameraSession = session;
        }

        void ToggleFlashMode()
        {
            if (CurrentFlashMode == AVCaptureFlashMode.Off)
                CurrentFlashMode = AVCaptureFlashMode.On;
            else if (CurrentFlashMode == AVCaptureFlashMode.On)
                CurrentFlashMode = AVCaptureFlashMode.Auto;
            else
                CurrentFlashMode = AVCaptureFlashMode.Off;

        }

        void UpdateFlashModeUI()
        {
            flashImageView.RemoveFromSuperview();
            flashImageView = CurrentFlashMode switch
            {
                AVCaptureFlashMode.Auto => flashButton.SetInsideImage(FLASH_AUTO),
                AVCaptureFlashMode.On => flashButton.SetInsideImage(FLASH),
                _ => flashButton.SetInsideImage(FLASH_OFF),
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        void SubscribeToEvents()
        {
            closeButton.TouchUpInside += CloseButton_TouchUpInside;
            shutterButton.TouchUpInside += ShutterButton_TouchUpInside;
            switchCameraButton.TouchUpInside += SwitchCameraButton_TouchUpInside;
            flashButton.TouchUpInside += FlashButton_TouchUpInside;
            PropertyChanged += OnPropertyChangedHere;
        }

        void UnSubscribeFromEvents()
        {
            closeButton.TouchUpInside -= CloseButton_TouchUpInside;
            shutterButton.TouchUpInside -= ShutterButton_TouchUpInside;
            switchCameraButton.TouchUpInside -= SwitchCameraButton_TouchUpInside;
            flashButton.TouchUpInside -= FlashButton_TouchUpInside;
            PropertyChanged -= OnPropertyChangedHere;
        }

        [Export("captureOutput:didFinishProcessingPhoto:error:")]
        public void DidFinishProcessingPhoto(AVCapturePhotoOutput output, AVCapturePhoto photo, NSError error)
        {
            var data = photo.FileDataRepresentation;
            InvokeOnMainThread(() =>
            {
                var controller = new ReelEditViewController(data);
                controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(controller, true, null);
            });
        }
        #endregion
    }
}

