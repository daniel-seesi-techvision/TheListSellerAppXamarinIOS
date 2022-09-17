using System;
using System.Collections.Generic;
using TheListSellerAppXamariniOS.Views.Products;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class Reel
    {
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<Product> LinkedProducts { get; set; }
    }
}

