using System;
using Foundation;
using UIKit;

namespace TheListSellerAppXamariniOS.Extensions
{
    public class ManipulatedString
    {
        public NSMutableAttributedString FormatedString { get; set; }

        #region private fields
        private static readonly NSMutableParagraphStyle textCenter = new NSMutableParagraphStyle() { Alignment = UITextAlignment.Center };
        private static readonly NSMutableParagraphStyle textLeft = new NSMutableParagraphStyle() { Alignment = UITextAlignment.Left };
        private static readonly NSMutableParagraphStyle textRigth = new NSMutableParagraphStyle() { Alignment = UITextAlignment.Right };
        private static readonly NSMutableParagraphStyle textJustified = new NSMutableParagraphStyle() { Alignment = UITextAlignment.Justified };


        private readonly UIStringAttributes underlineAttr = new UIStringAttributes { UnderlineStyle = NSUnderlineStyle.Single };
        private readonly UIStringAttributes stringCenterFormating = new UIStringAttributes { ParagraphStyle = textCenter };
        private readonly UIStringAttributes stringLeftFormating = new UIStringAttributes { ParagraphStyle = textLeft };
        private readonly UIStringAttributes stringRightFormating = new UIStringAttributes { ParagraphStyle = textRigth };
        private readonly UIStringAttributes stringFormating = new UIStringAttributes { ParagraphStyle = textJustified };
        #endregion

        public ManipulatedString(string stringToBeFormated)
        {
            FormatedString = new NSMutableAttributedString(stringToBeFormated);
        }

        /// <summary>
        /// Aligns string passed as parameter during class instantiation to alignment
        /// </summary>
        /// <param name="alignment">enum of possible alignments</param>
        public ManipulatedString AddAlignment(FormatAlignment alignment)
        {

            switch (alignment)
            {
                case FormatAlignment.Center:
                    FormatedString.AddAttributes(stringCenterFormating.Dictionary, new NSRange(0, FormatedString.Length));
                    break;
                case FormatAlignment.Left:
                    FormatedString.AddAttributes(stringLeftFormating.Dictionary, new NSRange(0, FormatedString.Length));
                    break;
                case FormatAlignment.Right:
                    FormatedString.AddAttributes(stringRightFormating.Dictionary, new NSRange(0, FormatedString.Length));
                    break;
                case FormatAlignment.Justified:
                    FormatedString.AddAttributes(stringFormating.Dictionary, new NSRange(0, FormatedString.Length));
                    break;
            }
            return this;

        }

        /// <summary>
        /// Underlines the whole string
        /// </summary>
        public ManipulatedString UnderLineText()
        {
            FormatedString.AddAttributes(underlineAttr, new NSRange(0, FormatedString.Length));
            return this;
        }

        /// <summary>
        /// Underlines the string starting from startIndex to the end of the lenght specified
        /// </summary>
        /// <param name="startIndex">char index at which underlining begins</param>
        /// <param name="lenght">Lenght of characters to be underlines</param>
        public ManipulatedString UnderLineText(int startIndex, int lenght)
        {
            FormatedString.AddAttributes(underlineAttr, new NSRange(startIndex, lenght));
            return this;
        }

        /// <summary>
        /// Sets font size of the whole string
        /// </summary>
        /// <param name="fontSize">int value of font size</param>
        public ManipulatedString SetFontSize(int fontSize)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { Font = UIFont.SystemFontOfSize(fontSize) };
            FormatedString.AddAttributes(stringFormating, new NSRange(0, FormatedString.Length));
            return this;
        }

        /// <summary>
        /// Sets font size of string starting from startIndex to the end of lenght specified
        /// </summary>
        /// <param name="fontSize">int value of font size</param>
        /// <param name="startIndex">char index of string at which font size format begins</param>
        /// <param name="lenght">Lenght of characters to which font size should apply</param>
        public ManipulatedString SetFontSize(int fontSize, int startIndex, int lenght)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { Font = UIFont.SystemFontOfSize(fontSize) };
            FormatedString.AddAttributes(stringFormating, new NSRange(startIndex, lenght));
            return this;
        }

        /// <summary>
        /// Sets font color to whole string
        /// </summary>
        /// <param name="fontColor">UIColor value for font color</param>
        public ManipulatedString SetFontColor(UIColor fontColor)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { ForegroundColor = fontColor };
            FormatedString.AddAttributes(stringFormating, new NSRange(0, FormatedString.Length));
            return this;
        }

        /// <summary>
        /// Sets font color of string starting from startIndex to the end of lenght specified
        /// </summary>
        /// <param name="fontColor">UIColor value for font color</param>
        /// <param name="startIndex">char index of string at which font color applies</param>
        /// <param name="lenght">Lenght of characters for which font color should apply</param>
        public ManipulatedString SetFontColor(UIColor fontColor, int startIndex, int lenght)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { ForegroundColor = fontColor };
            FormatedString.AddAttributes(stringFormating, new NSRange(startIndex, lenght));
            return this;
        }

        /// <summary>
        /// Sets string's font
        /// </summary>
        /// <param name="font">UIFont</param>
        public ManipulatedString SetFont(UIFont font)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { Font = font };
            FormatedString.AddAttributes(stringFormating, new NSRange(0, FormatedString.Length));
            return this;
        }

        /// <summary>
        /// Sets font to string starting from startIndex
        /// </summary>
        /// <param name="font">UIFont</param>
        /// <param name="startIndex">char index of string of which font applies</param>
        /// <param name="lenght">Lenght of characters for which font should apply</param>
        public ManipulatedString SetFont(UIFont font, int startIndex, int lenght)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { Font = font };
            FormatedString.AddAttributes(stringFormating, new NSRange(startIndex, lenght));
            return this;
        }

        public ManipulatedString AddLink(string linkUrl)
        {
            UIStringAttributes stringFormating = new UIStringAttributes { Link = new NSUrl(linkUrl) };
            FormatedString.AddAttributes(stringFormating, new NSRange(0, FormatedString.Length));
            return this;
        }

        public ManipulatedString AddLink(string linkUrl, int startIndex, int lenght)
        {
            FormatedString.AddAttribute(UIStringAttributeKey.Link, new NSUrl(linkUrl), new NSRange(startIndex, lenght));
            return this;
        }
        public enum FormatAlignment
        {
            Center,
            Left,
            Right,
            Justified
        };
    }

}

