using System;
using TheListSellerAppXamariniOS.Views.Reels;

namespace TheListSellerAppXamariniOS.Extensions
{
    public static class ReelExtensions
    {
        public static bool IsValid(this Reel reel) => !string.IsNullOrEmpty(reel.Description)
            && reel.LinkedProducts.Count > 0;
    }
}

