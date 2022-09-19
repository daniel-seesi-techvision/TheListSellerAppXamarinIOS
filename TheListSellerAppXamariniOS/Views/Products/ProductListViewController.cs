using System;
using System.Drawing.Printing;
using Foundation;
using TheListSellerAppXamariniOS.Extensions;
using UIKit;
using Xamarin.Forms;
using static TheListSellerAppXamariniOS.Constants.Dimensions;
using static TheListSellerAppXamariniOS.Constants.StringConstants;
using static TheListSellerAppXamariniOS.Constants.Colors;
using static TheListSellerAppXamariniOS.Constants.NotificationConstants;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using TheListSellerAppXamariniOS.Data.Repository;
using TheListSellerAppXamariniOS.DI;
using System.Collections.Generic;
using System.Linq;
using TheListSellerAppXamariniOS.Views.Reels;
using TheListSellerAppXamariniOS.Services;

namespace TheListSellerAppXamariniOS.Views.Products
{
    public class ProductListViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate, IUISearchBarDelegate
    {
        #region Fields
        IDataRepository<Product> dataRepository;
        private IAlertService alertService;
        List<Product> products;
        List<Product> filteredProducts;

        UIButton closeButton, saveButton;
        UISearchBar searchBar;
        UITableView productListTable;
        #endregion

        public List<Product> LinkedProducts { get; set; }

        #region Life Cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterServices();
            SetupUI();
            GetProducts();
            SetupTable();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SubcribeToEvents();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            LinkedProducts = products.Where(m => m.IsSelected).ToList();
            NSNotificationCenter.DefaultCenter.PostNotificationName((NSString)DONE_EDITING, this);
            UnSubcribeFromEvents();
        }
        #endregion

        #region Events

        private void CloseButton_TouchUpInside(object sender, EventArgs e)
        {
            DismissViewController(true, null);
        }

        private void SaveButton_TouchUpInside(object sender, EventArgs e)
        {
            SaveSelection();
        }
        #endregion

        #region Private Methods
        void RegisterServices()
        {
            dataRepository = IoC.Get<IDataRepository<Product>>();
            alertService = IoC.Get<IAlertService>();
        }

        void SetupUI()
        {
            View.BackgroundColor = UIColor.White;
            var margins = View.LayoutMarginsGuide;

            View.RemoveKeyboardOnTap();

            var titleLabel = new UILabel()
            {
                Text = "Add Products to Reel",
                TextColor = UIColor.Black,
                Lines = 2,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Semibold)
            };

            closeButton = new UIButton()
            {
                BackgroundColor = TransparentView,
                TintColor = UIColor.Black,
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            _ = closeButton.SetInsideImage(CLOSE_BLACK);

            searchBar = new UISearchBar() { Placeholder = SEARCH_PLACEHOLDER };
            searchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            searchBar.Delegate = this;
            productListTable = new UITableView() { BackgroundColor = TransparentView };
            saveButton = new UIButton()
            {
                TintColor = UIColor.White,
                BackgroundColor = UIColor.Black,
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Bold)

            };
            saveButton.SetTitle("SAVE SELECTION", UIControlState.Normal);
            saveButton.Layer.CornerRadius = 2F;

            View.AddViews(titleLabel, closeButton, productListTable, searchBar, productListTable, saveButton);

            titleLabel.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            titleLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            titleLabel.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.8F).Active = true;
            titleLabel.HeightAnchor.ConstraintEqualTo(50).Active = true;

            closeButton.TopAnchor.ConstraintEqualTo(margins.TopAnchor, 10).Active = true;
            closeButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor, -10).Active = true;
            closeButton.HeightAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
            closeButton.WidthAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;

            searchBar.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor, 10).Active = true;
            searchBar.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            searchBar.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            searchBar.HeightAnchor.ConstraintEqualTo(titleLabel.HeightAnchor).Active = true;

            productListTable.TopAnchor.ConstraintEqualTo(searchBar.BottomAnchor).Active = true;
            productListTable.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            productListTable.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            productListTable.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, CAMERA_ROOT_VIEW_BOTTOM_ANCHOR).Active = true;
            productListTable.RemoveKeyboardOnTap();

            saveButton.TopAnchor.ConstraintEqualTo(productListTable.BottomAnchor).Active = true;
            saveButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            saveButton.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor).Active = true;
            saveButton.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor).Active = true;
            saveButton.HeightAnchor.ConstraintEqualTo(FLOAT_BUTTON_SIZE).Active = true;
        }

        void GetProducts()
        {
            var list = dataRepository.FindAll();
            if (list.Count < 0)
            {
                //TODO make request to fecth products
            }
            products = list;
            filteredProducts = products;

            foreach (var item in filteredProducts)
            {
                item.IsSelected = LinkedProducts.Any(m => m.Id == item.Id);
            }
        }

        void SetupTable()
        {
            productListTable.DataSource = this;
            productListTable.Delegate = this;
            productListTable.RegisterClassForCellReuse(typeof(ProductCellView), PRODUCT_CELL_ID);
            productListTable.ShowsVerticalScrollIndicator = false;
            productListTable.RowHeight = 100;
            productListTable.ReloadData();
        }

        void SaveSelection()
        {
            LinkedProducts = products.Where(m => m.IsSelected).ToList();
            if (LinkedProducts.Count == 0)
            {
                alertService.ShowAlertAsync("Please select at least on product", "No Product Select");
                return;
            }

            DismissViewController(true,null);
        }

        void SubcribeToEvents()
        {
            closeButton.TouchUpInside += CloseButton_TouchUpInside;
            saveButton.TouchUpInside += SaveButton_TouchUpInside;
        }

        void UnSubcribeFromEvents()
        {
            closeButton.TouchUpInside -= CloseButton_TouchUpInside;
            saveButton.TouchUpInside -= SaveButton_TouchUpInside;
        }
        #endregion

        #region Delegate Methods

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(PRODUCT_CELL_ID, indexPath) as ProductCellView;
            cell.CheckBox.Checked = false;
            var asset = filteredProducts[indexPath.Row];
            cell.UpdateCell(asset);
            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as ProductCellView;

            // Toggle check box
            cell.CheckBox.Checked = !cell.CheckBox.Checked;

            // Updates check box UI
            cell.CheckBox.UpdateChecked();

            var product = filteredProducts[indexPath.Row];

            // Update product at source
            products.FirstOrDefault(m => m.Id == product.Id).IsSelected = cell.CheckBox.Checked;

            filteredProducts[indexPath.Row].IsSelected = cell.CheckBox.Checked;
        }

        public nint RowsInSection(UITableView tableView, nint section) => filteredProducts.Count;

        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 150F;

        [Export("searchBar:textDidChange:")]
        public void TextChanged(UISearchBar searchBar, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))            
                filteredProducts = products.Where(m => m.Designer.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)
                    || m.Title.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();            
            else
                filteredProducts = products;

            productListTable.ReloadData();
        }

        [Export("tableView:didEndEditingRowAtIndexPath:")]
        public void DidEndEditing(UITableView tableView, NSIndexPath indexPath)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

