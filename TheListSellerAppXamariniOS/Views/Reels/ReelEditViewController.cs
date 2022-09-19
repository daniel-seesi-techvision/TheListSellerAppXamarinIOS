using System;
using Foundation;
using ObjCRuntime;
using TheListSellerAppXamariniOS.Extensions;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using static TheListSellerAppXamariniOS.Constants.Dimensions;
using static TheListSellerAppXamariniOS.Constants.NotificationConstants;
using static TheListSellerAppXamariniOS.Constants.Colors;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using TheListSellerAppXamariniOS.Views.Products;
using TheListSellerAppXamariniOS.Data.Repository;
using TheListSellerAppXamariniOS.DI;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using TheListSellerAppXamariniOS.Services;
using CoreGraphics;
using TheListSellerAppXamariniOS.Constants;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelEditViewController : UIViewController, INotifyPropertyChanged, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
    {
        #region Fields
        NSObject doneEditingObserver;
        Action<NSNotification> doneEditingNotificationHandler;

        private readonly UIImage reelPhoto;

        const string descriptionLabelText = "Add Description";
        const string linkProductionLabelText = "Link Products";
        UIButton closeButton,
            linkProductButton,
            linkProductTitleButton,
            descriptionButton,
            descriptionTitleButton,
            previewButton,
            previewTitleButton,
            tagButton,
            tagTitleButton,
            uploadButton;
        UIView cameraRootView, productCollectionRootView;
        UICollectionView collectionView;
        UITextView descriptionTextView;

        bool _editing = false;
        Reel currentReel;
        IDataRepository<Reel> reelRepo;
        IDataRepository<ReelLinkedProduct> linkedReelProdudctRepo;
        IAlertService alertService;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        bool IsEditing
        {
            get => _editing;
            set
            {
                _editing = value;
                OnPropertyChanged();
            }
        }

        public ReelEditViewController(NSData data, Reel reel = null)
        {
            currentReel = reel ?? new();
            reelPhoto = UIImage.LoadFromData(data);
        }

        #region Life Cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterServices();
            SetupUI();
            doneEditingNotificationHandler = (NSNotification notification) =>
            {
                if (notification.Object is ReelDescriptionModal descriptionModal)
                {
                    ToggleFloatingButtons();
                    SaveDescription(descriptionModal.DescriptionText);
                }

                if (notification.Object is ProductListViewController productListViewController)
                {
                    currentReel.LinkedProducts = productListViewController.LinkedProducts;
                    UpdateLinkedProductLabelText(linkProductTitleButton, linkProductionLabelText);
                    UpdateUploadButtonState();
                    SetupProductCollection();
                }
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SubscribeToEvents();
            AddNotificationObservers();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UnSubscribeFromEvents();
            RemoveNotificationObservers();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetupProductCollection();
        }
        #endregion

        #region Events
        private void OnPropertyChangedHere(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsEditing))
            {
                InvokeOnMainThread(() => ToggleFloatingButtons());
            }
            if (e.PropertyName == nameof(currentReel.Description))
            {
                UpdateDescription(descriptionTitleButton, descriptionLabelText);
            }
        }

        private void CloseButton_TouchUpInside(object sender, EventArgs e) => DismissModalViewController(true);

        private void DescriptionButton_TouchUpInside(object sender, EventArgs e)
        {
            IsEditing = true;
            var controller = new ReelDescriptionModal();
            controller.DescriptionText = currentReel.Description;
            controller.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            PresentViewController(controller, true, null);
        }

        private void LinkProductButton_TouchUpInside(object sender, EventArgs e)
        {
            var controller = new ProductListViewController();
            controller.LinkedProducts = currentReel.LinkedProducts;
            PresentViewController(controller, true, null);
        }

        private void PreviewButton_TouchUpInside(object sender, EventArgs e)
        {
            TogglePreview(descriptionButton.Hidden);
        }

        private async void UploadButton_TouchUpInside(object sender, EventArgs e)
        {
            await SaveReel();
        }
        #endregion

        #region Methods
        void RegisterServices()
        {
            reelRepo = IoC.Get<IDataRepository<Reel>>();
            linkedReelProdudctRepo = IoC.Get<IDataRepository<ReelLinkedProduct>>();
            alertService = IoC.Get<IAlertService>();
        }

        void SetupUI()
        {
            View.BackgroundColor = UIColor.Black;
            var margins = View.LayoutMarginsGuide;
            cameraRootView = View.CreateCameraRootView();

            var imageView = new UIImageView(reelPhoto) { ContentMode = UIViewContentMode.ScaleAspectFill, ClipsToBounds = true };
            imageView.Layer.CornerRadius = CAMERA_ROOT_VIEW_CORNER_RADIUS;
            cameraRootView.AddView<UIImageView>(imageView);

            descriptionTextView = new UITextView()
            {
                Text = currentReel.Description,
                TextColor = UIColor.White,
                BackgroundColor = TransparentView,
                ScrollEnabled = false,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular)
            };
            cameraRootView.AddSubview(descriptionTextView);
            descriptionTextView.Editable = false;
            descriptionTextView.UserInteractionEnabled = false;
            descriptionTextView.TranslatesAutoresizingMaskIntoConstraints = false;
            descriptionTextView.BottomAnchor.ConstraintEqualTo(cameraRootView.BottomAnchor).Active = true;
            descriptionTextView.LeadingAnchor.ConstraintEqualTo(cameraRootView.LeadingAnchor, 10).Active = true;
            descriptionTextView.TrailingAnchor.ConstraintEqualTo(cameraRootView.TrailingAnchor, -10).Active = true;
            descriptionTextView.HeightAnchor.ConstraintEqualTo(cameraRootView.HeightAnchor, 0.1F).Active = true;

            var uploadPostInstructionLabel = new UILabel()
            {
                Text = "Add a description & minimum of 3 tags to upload post",
                Lines = 3,
                LineBreakMode = UILineBreakMode.WordWrap,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.White,
            };
            View.AddSubview(uploadPostInstructionLabel);
            uploadPostInstructionLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            uploadPostInstructionLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.4F).Active = true;
            uploadPostInstructionLabel.TopAnchor.ConstraintEqualTo(cameraRootView.BottomAnchor).Active = true;
            uploadPostInstructionLabel.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            uploadPostInstructionLabel.BottomAnchor.ConstraintEqualTo(margins.BottomAnchor).Active = true;

            uploadButton = new UIButton()
            {
                TintColor = UIColor.LightGray,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Bold)
            };
            View.AddSubview(uploadButton);
            uploadButton.SetTitle("UPLOAD", UIControlState.Normal);
            uploadButton.SetTitleColor(UIColor.LightGray, UIControlState.Disabled);
            uploadButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            uploadButton.BackgroundColor = TransparentView;
            uploadButton.TranslatesAutoresizingMaskIntoConstraints = false;
            uploadButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            uploadButton.Layer.BorderWidth = 1;
            uploadButton.Layer.CornerRadius = FLOAT_BUTTON_SIZE / 2;
            var image = new UIImage(SMALL_CHECKED);
            uploadButton.SetImage(image, UIControlState.Normal);

            var widthEstimate = View.Frame.Size.Width / 2;
            uploadButton.ImageEdgeInsets = new UIEdgeInsets(0, (widthEstimate / 2) + image.Size.Width + 10, 0, 0);
            uploadButton.TitleEdgeInsets = new UIEdgeInsets(0, 0, 0, (widthEstimate / 2) - (image.Size.Width + 20));

            uploadButton.CenterYAnchor.ConstraintEqualTo(uploadPostInstructionLabel.CenterYAnchor).Active = true;
            uploadButton.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5F).Active = true;
            uploadButton.HeightAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            uploadButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            uploadButton.Enabled = false;

            closeButton = cameraRootView.CreateCloseButton();

            linkProductButton = SetUpFloatingButton(HANGER);
            linkProductTitleButton = SetupFloatingButtonTitle(linkProductButton, linkProductionLabelText);
            linkProductButton.CenterYAnchor.ConstraintEqualTo(cameraRootView.CenterYAnchor).Active = true;

            tagButton = SetUpFloatingButton(TAG);
            tagTitleButton = SetupFloatingButtonTitle(tagButton, "Add Tags");
            tagButton.BottomAnchor.ConstraintEqualTo(linkProductButton.TopAnchor, -15).Active = true;

            descriptionButton = SetUpFloatingButton(DESCRIPTION);
            descriptionTitleButton = SetupFloatingButtonTitle(descriptionButton, descriptionLabelText);
            descriptionButton.BottomAnchor.ConstraintEqualTo(tagButton.TopAnchor, -15).Active = true;

            previewButton = SetUpFloatingButton(EYE);
            previewTitleButton = SetupFloatingButtonTitle(previewButton, "View Preview");
            previewButton.TopAnchor.ConstraintEqualTo(linkProductButton.BottomAnchor, 15).Active = true;

            productCollectionRootView = new UIView() { BackgroundColor = TransparentView };
            cameraRootView.AddSubview(productCollectionRootView);
            productCollectionRootView.TranslatesAutoresizingMaskIntoConstraints = false;
            productCollectionRootView.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            productCollectionRootView.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            productCollectionRootView.BottomAnchor.ConstraintEqualTo(descriptionTextView.TopAnchor).Active = true;
            productCollectionRootView.HeightAnchor.ConstraintEqualTo(70).Active = true;
        }

        UIButton SetUpFloatingButton(string image)
        {
            var button = new UIButton() { BackgroundColor = TransparentButton, TintColor = UIColor.White, ContentMode = UIViewContentMode.Center };
            cameraRootView.AddSubview(button);
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            _ = button.SetInsideImage(image);
            button.Layer.CornerRadius = FLOAT_BUTTON_CORNER_RADIUS;
            button.HeightAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            button.WidthAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            button.TrailingAnchor.ConstraintEqualTo(cameraRootView.TrailingAnchor, -20).Active = true;

            return button;
        }

        UIButton SetupFloatingButtonTitle(UIButton button, string text)
        {
            var titleButton = new UIButton()
            {
                TintColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold),
                BackgroundColor = TransparentView
            };
            titleButton.SetTitle(text, UIControlState.Normal);
            titleButton.Enabled = false;
            titleButton.UserInteractionEnabled = false;
            titleButton.TranslatesAutoresizingMaskIntoConstraints = false;
            cameraRootView.AddSubview(titleButton);
            titleButton.CenterYAnchor.ConstraintEqualTo(button.CenterYAnchor).Active = true;
            titleButton.TrailingAnchor.ConstraintEqualTo(button.LeadingAnchor, -20).Active = true;
            return titleButton;
        }

        void SubscribeToEvents()
        {
            closeButton.TouchUpInside += CloseButton_TouchUpInside;
            descriptionButton.TouchUpInside += DescriptionButton_TouchUpInside;
            linkProductButton.TouchUpInside += LinkProductButton_TouchUpInside;
            previewButton.TouchUpInside += PreviewButton_TouchUpInside;
            uploadButton.TouchUpInside += UploadButton_TouchUpInside;
            PropertyChanged += OnPropertyChangedHere;
            currentReel.PropertyChanged += OnPropertyChangedHere;
        }

        void UnSubscribeFromEvents()
        {
            closeButton.TouchUpInside -= CloseButton_TouchUpInside;
            descriptionButton.TouchUpInside -= DescriptionButton_TouchUpInside;
            previewButton.TouchUpInside -= PreviewButton_TouchUpInside;
            linkProductButton.TouchUpInside -= LinkProductButton_TouchUpInside;
            PropertyChanged -= OnPropertyChangedHere;
            currentReel.PropertyChanged -= OnPropertyChangedHere;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        void ToggleFloatingButtons()
        {
            var visibility = descriptionButton.Hidden;
            TogglePreview(visibility);
            previewButton.Hidden = !visibility;
            previewTitleButton.Hidden = !visibility;
        }

        void TogglePreview(bool visibility)
        {
            descriptionButton.Hidden = !visibility;
            descriptionTitleButton.Hidden = !visibility;
            linkProductButton.Hidden = !visibility;
            linkProductTitleButton.Hidden = !visibility;
            tagButton.Hidden = !visibility;
            tagTitleButton.Hidden = !visibility;
        }

        void AddNotificationObservers()
        {
            doneEditingObserver = NSNotificationCenter.DefaultCenter.AddObserver((NSString)DONE_EDITING, null, NSOperationQueue.MainQueue, doneEditingNotificationHandler);
        }

        void RemoveNotificationObservers()
        {
            if (doneEditingObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(doneEditingObserver);
        }

        void UpdateDescription(UIButton button, string defaultText)
        {
            InvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(currentReel.Description))
                {
                    button.SetTitle(defaultText, UIControlState.Normal);
                }
                else
                {
                    button.SetTitle(null, UIControlState.Normal);
                    button.SetInsideImage(SMALL_CHECKED);
                }
                descriptionTextView.Text = currentReel.Description;
                UpdateUploadButtonState();
            });
        }

        void UpdateLinkedProductLabelText(UIButton button, string defaultText)
        {
            InvokeOnMainThread(() =>
            {
                if (currentReel.LinkedProducts.Count == 0)
                {
                    button.SetTitle(defaultText, UIControlState.Normal);
                }
                else
                {
                    button.SetTitle(null, UIControlState.Normal);
                    button.SetInsideImage(SMALL_CHECKED);
                }
            });
        }

        void UpdateUploadButtonState()
        {
            if (currentReel.IsValid())
            {
                uploadButton.Enabled = true;
                uploadButton.BackgroundColor = UIColor.White;
                uploadButton.SetImage(new UIImage(SMALL_BLACK_CHECKED), UIControlState.Normal);
                uploadButton.TintColor = UIColor.Black;
                uploadButton.Layer.BorderWidth = 0;
            }
            else
            {
                uploadButton.Enabled = false;
                uploadButton.BackgroundColor = TransparentView;
                uploadButton.TintColor = UIColor.LightGray;
                uploadButton.Layer.BorderWidth = 1;
                uploadButton.SetImage(new UIImage(SMALL_CHECKED), UIControlState.Normal);
            }
        }

        void SaveDescription(string descriptionText)
        {
            currentReel.Description = descriptionText == REEL_DESCRIPTION_PLACEHOLDER ? "" : descriptionText;
        }

        async Task SaveReel()
        {
            try
            {
                alertService.ShowLoading("Uploading..");

                using var stream = reelPhoto.AsJPEG().AsStream();
                string PersonalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = Guid.NewGuid().ToString();
                string filePath = Path.Combine(PersonalFolderPath, fileName);

                using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create))
                    await stream.CopyToAsync(outputFileStream);

                var reel = new Reel()
                {
                    CreateAt = DateTime.UtcNow,
                    Description = currentReel.Description,
                    ImageUrl = fileName
                };

                await reelRepo.CreateAsync(reel);

                var reelLinkedProducts = currentReel.LinkedProducts.Select(m => new ReelLinkedProduct()
                {
                    ProductId = m.Id,
                    ReelId = reel.Id
                });

                await linkedReelProdudctRepo.CreateMultipleAsync(reelLinkedProducts);
                View.Window.RootViewController.DismissViewController(true, null);
            }
            catch (Exception ex)
            {
                alertService.HideLoading();
                await alertService.ShowAlertAsync(ex.Message, "Error Saving Reel");
            }
            finally
            {
                alertService.HideLoading();
            }
        }

        void SetupProductCollection()
        {
            var collectionLayout = new UICollectionViewFlowLayout();
            collectionLayout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            collectionLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            collectionView = new UICollectionView(productCollectionRootView.Bounds, collectionLayout) { BackgroundColor = TransparentView };

            collectionView.Delegate = this;
            collectionView.DataSource = this;
            collectionView.RegisterClassForCell(typeof(ReelCellView), REEL_CELL_ID);

            productCollectionRootView.AddSubview(collectionView);
            collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            collectionView.ReloadData();
        }
        #endregion

        #region Delegate Methods
        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return currentReel.LinkedProducts.Count;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(StringConstants.REEL_CELL_ID, indexPath) as ReelCellView;
            var reel = currentReel.LinkedProducts[indexPath.Row];
            cell.LoadImage(reel.ImageUrl);
            return cell;
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var lay = layout as UICollectionViewFlowLayout;
            var widthPerItem = (collectionView.Frame.Width / 4) - lay.MinimumInteritemSpacing;
            return new CGSize(widthPerItem, 50);
        }
        #endregion
    }
}

