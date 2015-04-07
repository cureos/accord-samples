// Copyright (c) 2014-2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace IPPrototyper.Core.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public interface IImageLoaderService
    {
        #region METHODS

        Task<IEnumerable<Bitmap>> LoadImagesAsync();

        Task<Bitmap> CaptureImageAsync();

        #endregion
    }
}