using System;
using System.Collections.Generic;
using SQLite;
using TheListSellerAppXamariniOS.Views.Products;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class Reel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<ReelLinkedProduct> LinkedProducts { get; set; }
    }
}

