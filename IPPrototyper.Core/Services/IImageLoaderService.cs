// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageLoaderService.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IPPrototyper.Core.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    public interface IImageLoaderService
    {
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        Task<IEnumerable<Bitmap>> LoadImagesAsync();
    }
}