// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Corners.SURF
{
    using System;
    using System.Drawing;
    using System.IO;

    using Accord.Imaging;
    using Accord.Imaging.Filters;

    using Windows.ApplicationModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private Bitmap lenna;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Resources");
            var file = await folder.GetFileAsync("lena512.bmp");

            using (var stream = await file.OpenReadAsync())
            {
                this.lenna = (Bitmap)Image.FromStream(stream.AsStreamForRead());
            }

            this.LenaImage.Source = (BitmapSource)this.lenna;
        }

        private void DetectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threshold = (float)Math.Pow(10.0, this.LogThresholdSlider.Value);
            var octaves = (int)this.OctaveSlider.Value;
            var initial = (int)this.InitialSlider.Value;

            // Create a new SURF Features Detector using the given parameters
            var surf = new SpeededUpRobustFeaturesDetector(threshold, octaves, initial);

            var points = surf.ProcessImage(this.lenna);

            // Create a new AForge's Corner Marker Filter
            var features = new FeaturesMarker(points);

            // Apply the filter and display it on a picturebox
            this.LenaImage.Source = (BitmapSource)features.Apply(this.lenna);
        }
    }
}
