using System;
using System.Collections.Generic;
using System.IO;
using CoreGraphics;
using FFImageLoading;
using Foundation;
using TheListSellerAppXamariniOS.Constants;
using UIKit;
using static TheListSellerAppXamariniOS.Constants.Colors;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelCollectionLayoutDelegate : UICollectionViewDelegateFlowLayout
    {
        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var lay = layout as UICollectionViewFlowLayout;
            var widthPerItem = (collectionView.Frame.Width / 2) - lay.MinimumInteritemSpacing;
            return new CGSize(widthPerItem, 200);
        }
    }

    public class ReelCollectionViewDataSource : UICollectionViewDataSource
    {
        public List<Reel> Reels { get; set; } = new();


        public ReelCollectionViewDataSource(List<Reel> reels)
        {
            string PersonalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            foreach (var reel in reels)
            {

                string filePath = Path.Combine(PersonalFolderPath, reel.ImageUrl);
                reel.ImageUrl = filePath;
                Reels.Add(reel);
            }

        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(StringConstants.REEL_CELL_ID, indexPath) as ReelCellView;
            var reel = Reels[indexPath.Row];
            cell.SetImage(reel.ImageUrl);
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Reels.Count;
    }

    public class ReelCellView : UICollectionViewCell
    {
        private UIImageView imageView;

        public ReelCellView(IntPtr handle) : base(handle)
        {
            SetUpView();
        }

        void SetUpView()
        {
            BackgroundColor = TransparentView;
            imageView = new UIImageView();

            AddSubview(imageView);
            imageView.ClipsToBounds = true;
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            imageView.TopAnchor.ConstraintEqualTo(this.TopAnchor).Active = true;
            imageView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).Active = true;
            imageView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor).Active = true;
            imageView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor).Active = true;
        }

        public void SetImage(string imageUrl)
        {
            ImageService.Instance.LoadFile(imageUrl)
                              .LoadingPlaceholder("placeholder.png")
                              .Retry(3, 2000)
                              .Into(imageView);
        }

        public void LoadImage(string imageUrl)
        {
            ImageService.Instance.LoadUrl(imageUrl)
                              .LoadingPlaceholder("placeholder.png")
                              .Retry(3, 2000)
                              .WithCache(FFImageLoading.Cache.CacheType.Disk)
                              .Into(imageView);
        }
    }
}

