﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        #endregion array

        #region Bitmap

        public static BitmapImage BitmapToImageSource(this Bitmap bitmap, System.Drawing.Imaging.ImageFormat format, bool freeze = true)
        {
            using MemoryStream memory = new MemoryStream();
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

        #endregion Bitmap

        public static void Swap<T>(this IList<T> list, T obj1, T obj2)
        {
            var indexA = list.IndexOf(obj1);
            var indexB = list.IndexOf(obj2);
            list[indexA] = obj2;
            list[indexB] = obj1;
        }
    }
}