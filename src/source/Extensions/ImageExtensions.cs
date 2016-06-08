using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chronos.Extensions
{
    public static class ImageExtensions
    {
        #region · Extensions ·

        /// <summary>
        /// Resizes the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Image Resize(this Image image, Size size)
        {
            Size newSize = new Size(size.Width, size.Height);

            if (image.Size.Width > image.Size.Height)
            {
                newSize.Height = (image.Size.Height * size.Width) / image.Size.Width;
            }
            else
            {
                newSize.Width = (image.Size.Width * size.Height) / image.Size.Height;
            }

            Rectangle   rectangle   = new Rectangle(0, 0, newSize.Width, newSize.Height);
            Image       resized     = new Bitmap(newSize.Width, newSize.Height, image.PixelFormat);

            using (Graphics graphic = Graphics.FromImage(resized))
            {
                graphic.CompositingQuality  = CompositingQuality.HighQuality;
                graphic.SmoothingMode       = SmoothingMode.HighQuality;
                graphic.InterpolationMode   = InterpolationMode.HighQualityBicubic;

                graphic.DrawImage((System.Drawing.Image)image.Clone(), rectangle);
            }

            return resized;
        }

        #endregion
    }
}
