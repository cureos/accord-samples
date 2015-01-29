// Copyright (c) 2010-2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using AForge;

namespace Panorama
{
    using System.Windows.Media;

    public partial class MainPage
    {
        private Bitmap img1;
        private Bitmap img2;

        private IntPoint[] harrisPoints1;
        private IntPoint[] harrisPoints2;

        private IntPoint[] correlationPoints1;
        private IntPoint[] correlationPoints2;

        private MatrixH homography;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var bmpi = new BitmapImage();

            bmpi.SetSource(Application.GetResourceStream(new Uri(@"Resources/UFSCar_Lake1.png", UriKind.Relative)).Stream);
            img1 = ((Bitmap)new WriteableBitmap(bmpi)).Clone(PixelFormat.Format24bppRgb);

            bmpi.SetSource(Application.GetResourceStream(new Uri(@"Resources/UFSCar_Lake2.png", UriKind.Relative)).Stream);
            img2 = ((Bitmap)new WriteableBitmap(bmpi)).Clone(PixelFormat.Format24bppRgb);

            // Concatenate and show entire image at start
            var concatenate = new Concatenate(img1);
            PictureBox.Source = (ImageSource)concatenate.Apply(img2);
        }

        private void BtnHarris_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 1: Detect feature points using Harris Corners Detector
            var harris = new HarrisCornersDetector(0.04f, 1000f);
            harrisPoints1 = harris.ProcessImage(img1).ToArray();
            harrisPoints2 = harris.ProcessImage(img2).ToArray();

            // Show the marked points in the original images
            var img1mark = new PointsMarker(harrisPoints1).Apply(img1);
            var img2mark = new PointsMarker(harrisPoints2).Apply(img2);

            // Concatenate the two images together in a single image (just to show on screen)
            var concatenate = new Concatenate(img1mark);
            PictureBox.Source = (ImageSource)concatenate.Apply(img2mark);
        }

        private void BtnCorrelation_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 2: Match feature points using a correlation measure
            var matcher = new CorrelationMatching(9, img1, img2);
            var matches = matcher.Match(harrisPoints1, harrisPoints2);

            // Get the two sets of points
            correlationPoints1 = matches[0];
            correlationPoints2 = matches[1];

            // Concatenate the two images in a single image (just to show on screen)
            var concat = new Concatenate(img1);
            var img3 = concat.Apply(img2);

            // Show the marked correlations in the concatenated image
            var pairs = new PairsMarker(
                correlationPoints1, // Add image1's width to the X points to show the markings correctly
                correlationPoints2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            PictureBox.Source = (ImageSource)pairs.Apply(img3);
        }

        private void BtnRansac_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 3: Create the homography matrix using a robust estimator
            var ransac = new RansacHomographyEstimator(0.001, 0.99);
            homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            // Plot RANSAC results against correlation results
            var inliers1 = correlationPoints1.Submatrix(ransac.Inliers);
            var inliers2 = correlationPoints2.Submatrix(ransac.Inliers);

            // Concatenate the two images in a single image (just to show on screen)
            var concat = new Concatenate(img1);
            var img3 = concat.Apply(img2);

            // Show the marked correlations in the concatenated image
            var pairs = new PairsMarker(
                inliers1, // Add image1's width to the X points to show the markings correctly
                inliers2.Apply(p => new IntPoint(p.X + img1.Width, p.Y)));

            PictureBox.Source = (ImageSource)pairs.Apply(img3);
        }

        private void BtnBlend_OnClick(object sender, RoutedEventArgs e)
        {
            // Step 4: Project and blend the second image using the homography
            var blend = new Blend(homography, img1);
            PictureBox.Source = (ImageSource)blend.Apply(img2);
        }

        private void BtnDoItAll_OnClick(object sender, RoutedEventArgs e)
        {
            // Do it all
            BtnHarris_OnClick(sender, e);
            BtnCorrelation_OnClick(sender, e);
            BtnRansac_OnClick(sender, e);
            BtnBlend_OnClick(sender, e);

        }
    }
}