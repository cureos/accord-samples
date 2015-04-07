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
    using Windows.Storage.Pickers;
    using Windows.UI.Xaml.Media.Imaging;

    public class ImageLoaderService : IImageLoaderService
    {
        #region METHODS

        public async Task<IEnumerable<Bitmap>> LoadImagesAsync()
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".bmp");
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");

            var selectedFolder = await folderPicker.PickSingleFolderAsync();
            var selectedFiles = await selectedFolder.GetFilesAsync();

            var bitmaps = new List<Bitmap>();
            foreach (var file in selectedFiles)
            {
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

        public Task<Bitmap> CaptureImageAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}