using SQLite;

namespace TheListSellerAppXamariniOS.Views.Reels
{
    public class ReelLinkedProduct
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ReelId { get; set; }
    }
}

