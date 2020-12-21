using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace UMLGenerator
{
    public static class Extensions
    {
        #region array
        public static bool Contains(this Array arr, object item)
        {
            return Array.IndexOf(arr, item) > -1;
        }
        #endregion

        #region Bitmap
        public static BitmapImage BitmapToImageSource(this Bitmap bitmap, System.Drawing.Imaging.ImageFormat format, bool freeze = true)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, format);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                if (freeze)
                    bitmapimage.Freeze();
                return bitmapimage;
            }
        }
        #endregion
    }
}
