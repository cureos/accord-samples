// Copyright (c) 2014-2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace Panorama
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using Accord;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math;

    using Windows.ApplicationModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// </summary>
        private Bitmap img1;

        /// <summary>
        /// </summary>
        private Bitmap img2;

        /// <summary>
        /// </summary>
        private IntPoint[] harrisPoints1;

        /// <summary>
        /// </summary>
        private IntPoint[] harrisPoints2;

        /// <summary>
        /// </summary>
        private IntPoint[] correlationPoints1;

        /// <summary>
        /// </summary>
        private IntPoint[] correlationPoints2;

        /// <summary>
        /// </summary>
        private MatrixH homography;

        /// <summary>
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Images");

            var file = await folder.GetFileAsync("UFSCar_Lake1.png");
            using (var stream = await file.OpenReadAsync())
            {
                this.img1 = ((Bitmap)System.Drawing.Image.FromStream(stream.AsStreamForRead())).Clone(PixelFormat.Format24bppRgb);
            }

            file = await folder.GetFileAsync("UFSCar_Lake2.png");
            using (var stream = await file.OpenReadAsync())
            {
                this.img2 = ((Bitmap)System.Drawing.Image.FromStream(stream.AsStreamForRead())).Clone(PixelFormat.Format24bppRgb);
            }

            // Concatenate and show entire image at start
            var concatenate = new Concatenate(this.img1);
            this.PictureBox.Source = (ImageSource)concatenate.Apply(this.img2);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void BtnHarris_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 1: Detect feature points using Harris Corners Detector
            var harris = new HarrisCornersDetector(0.04f, 1000f);
            this.harrisPoints1 = harris.ProcessImage(this.img1).ToArray();
            this.harrisPoints2 = harris.ProcessImage(this.img2).ToArray();

            // Show the marked points in the original images
            var img1mark = new PointsMarker(this.harrisPoints1).Apply(this.img1);
            var img2mark = new PointsMarker(this.harrisPoints2).Apply(this.img2);

            // Concatenate the two images together in a single image (just to show on screen)
            var concatenate = new Concatenate(img1mark);
            this.PictureBox.Source = (ImageSource)concatenate.Apply(img2mark);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void BtnCorrelation_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 2: Match feature points using a correlation measure
            var matcher = new CorrelationMatching(9, this.img1, this.img2);
            var matches = matcher.Match(this.harrisPoints1, this.harrisPoints2);

            // Get the two sets of points
            this.correlationPoints1 = matches[0];
            this.correlationPoints2 = matches[1];

            // Concatenate the two images in a single image (just to show on screen)
            var concat = new Concatenate(this.img1);
            var img3 = concat.Apply(this.img2);

            // Show the marked correlations in the concatenated image
            var pairs = new PairsMarker(this.correlationPoints1, // Add image1's width to the X points to show the markings correctly
                this.correlationPoints2.Apply(p => new IntPoint(p.X + this.img1.Width, p.Y)));

            this.PictureBox.Source = (ImageSource)pairs.Apply(img3);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void BtnRansac_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 3: Create the homography matrix using a robust estimator
            var ransac = new RansacHomographyEstimator(0.001, 0.99);
            this.homography = ransac.Estimate(this.correlationPoints1, this.correlationPoints2);

            // Plot RANSAC results against correlation results
            var inliers1 = this.correlationPoints1.Submatrix(ransac.Inliers);
            var inliers2 = this.correlationPoints2.Submatrix(ransac.Inliers);

            // Concatenate the two images in a single image (just to show on screen)
            var concat = new Concatenate(this.img1);
            var img3 = concat.Apply(this.img2);

            // Show the marked correlations in the concatenated image
            var pairs = new PairsMarker(
                inliers1, // Add image1's width to the X points to show the markings correctly
                inliers2.Apply(p => new IntPoint(p.X + this.img1.Width, p.Y)));

            this.PictureBox.Source = (ImageSource)pairs.Apply(img3);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void BtnBlend_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 4: Project and blend the second image using the homography
            var blend = new Blend(this.homography, this.img1);
            this.PictureBox.Source = (ImageSource)blend.Apply(this.img2);
        }
    }
}
