using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoreGraphics;
using UIKit;
using TheListSellerAppXamariniOS.Extensions;
using static TheListSellerAppXamariniOS.Constants.Colors;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.Views.Custom
{
    public class CheckBox : UIView, INotifyPropertyChanged
    {
        bool _checked = false;
        UIButton button;
        UIImageView backgroundImageView;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }

        public CheckBox(bool? isChecked = null)
        {
            Checked = isChecked ?? false;
            button = new UIButton();
            this.AddView<UIButton>(button);
            Layer.CornerRadius = 2F;
            Layer.BorderWidth = 1;
            Layer.BorderColor = new UIColor(red: 0.825F, green: 0.825F, blue: 0.825F, alpha: 1).CGColor;
            button.TouchUpInside += (sender, e) =>
            {
                Checked = !Checked;
                UpdateChecked();
            };

        }

        public void UpdateChecked()
        {            
            InvokeOnMainThread(() =>
            {
                if (Checked)
                {
                    button.BackgroundColor = UIColor.Black;
                    backgroundImageView = button.SetInsideImage(SMALL_CHECKED);
                }
                else
                {
                    button.BackgroundColor = TransparentView;
                    button.SetBackgroundImage(null, UIControlState.Normal);
                    if (backgroundImageView is not null)
                        backgroundImageView.RemoveFromSuperview();
                }
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

