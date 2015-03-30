// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageLoaderService.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GlyphRecognition.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage.Pickers;
    using Windows.Storage.Search;

    using IPPrototyper.Core.Services;

    /// <summary>
    /// </summary>
    public class ImageLoaderService : IImageLoaderService
    {
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<IEnumerable<Bitmap>> LoadImagesAsync()
        {
            var folderPicker = new FolderPicker { FileTypeFilter = { ".bmp", ".jpg", ".jpeg", ".png" } };
            var selectedFolder = await folderPicker.PickSingleFolderAsync();
            var selectedFiles = await selectedFolder.GetFilesAsync();

            var bitmaps = new List<Bitmap>();
            foreach (var file in selectedFiles)
            {
                var stream = await file.OpenStreamForReadAsync();
                try
                {
                    var bitmap = (Bitmap)Image.FromStream(stream);
                    bitmaps.Add(bitmap);
                }
                catch
                {
                }
            }

            return bitmaps;
        }
    }
}