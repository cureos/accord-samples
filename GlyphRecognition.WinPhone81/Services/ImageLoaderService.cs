// Copyright (c) 2014-2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace GlyphRecognition.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    using IPPrototyper.Core.Services;

    using Windows.Storage;
    using Windows.UI.Xaml.Media.Imaging;

    public class ImageLoaderService : IImageLoaderService
    {
        #region METHODS

        public async Task<IEnumerable<Bitmap>> LoadImagesAsync()
        {
            var bitmaps = new List<Bitmap>();
            for (var i = 11; i <= 20; ++i)
            {
                var uri = new Uri(string.Format("ms-appx:///Images/{0}.jpg", i));
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

                var stream = await file.OpenAsync(FileAccessMode.Read);
                try
                {
                    WriteableBitmap writeableBitmap = null;
                    var bitmap = (Bitmap)(await writeableBitmap.FromStream(stream));
                    bitmaps.Add(bitmap);
                }
                catch
                {
                }
            }

            return bitmaps;
        }

        #endregion
    }
}