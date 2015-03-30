namespace IPPrototyper.Core.Services
{
    using AForge.Imaging.IPPrototyper;

    public interface IImageProcessingService
    {
        IImageProcessingRoutine Current { get; } 
    }
}