namespace GlyphRecognition
{
    using System.Linq;
    using System.Reflection;

    using Windows.UI.Xaml.Controls;

    using Cirrious.CrossCore;
    using Cirrious.CrossCore.Platform;
    using Cirrious.MvvmCross.ViewModels;
    using Cirrious.MvvmCross.WindowsCommon.Platform;

    using GlyphRecognition.Services;

    using IPPrototyper.Core.Services;

    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new IPPrototyper.Core.App();
        }
        
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {
            Mvx.LazyConstructAndRegisterSingleton<IImageLoaderService, ImageLoaderService>();
            Mvx.LazyConstructAndRegisterSingleton<IImageProcessingService, GlyphRecognitionService>();
            base.InitializeFirstChance();
        }
    }
}