using System;
using SQLite;

namespace TheListSellerAppXamariniOS.Views.Products
{
    public class Product
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Designer { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}

