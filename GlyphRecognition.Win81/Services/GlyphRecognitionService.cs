namespace GlyphRecognition.Services
{
    using AForge.Imaging.IPPrototyper;

    using IPPrototyper.Core.Services;

    public class GlyphRecognitionService : IImageProcessingService
    {
        #region FIELDS

        private readonly IImageProcessingRoutine current = new GlyphRecognizer();

        #endregion

        #region PROPERTIES

        public IImageProcessingRoutine Current
        {
            get
            {
                return this.current;
            }
        }

        #endregion
    }
}