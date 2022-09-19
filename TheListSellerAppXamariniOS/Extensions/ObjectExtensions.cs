using System;
using Foundation;
using static System.Net.Mime.MediaTypeNames;

namespace TheListSellerAppXamariniOS.Extensions
{
    public static class ObjectExtensions
    {
        static void DelaySearch(this NSObject nsObject,string text, Action action, double afterDelay = 0.5)
        {
            //NSObject.CancelPreviousPerformRequest(nsObject);
            //perform(action, with: text, afterDelay: afterDelay)
        }
    }
}

