using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using TheListSellerAppXamariniOS.Views.Products;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class Reel : INotifyPropertyChanged
    {
        string _description;

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }

        public string ImageUrl {
            get; set; }

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                    return;
                _description = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public List<Product> LinkedProducts { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

