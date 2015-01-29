// Copyright (c) 2010-2015 Anders Gustafsson, Cureos AB.
// All rights reserved. 

using System;
using System.Collections;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;

namespace FaceDetection
{
    using System.Windows.Media;

    using Color = System.Drawing.Color;

    public partial class MainPage
    {
        private Bitmap picture;
        private HaarObjectDetector detector;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        public static ICollection SearchModes
        {
            get { return Enum.GetValues(typeof (ObjectDetectorSearchMode)); }
        }

        public static ICollection ScalingModes
        {
            get { return Enum.GetValues(typeof (ObjectDetectorScalingMode)); }
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var bmpi = new BitmapImage();
            bmpi.SetSource(Application.GetResourceStream(new Uri(@"Resources/judybats.jpg", UriKind.Relative)).Stream);
            pictureBox1.Source = new WriteableBitmap(bmpi);

            // Explicit cast from ImageSource to System.Drawing.Bitmap
            picture = (Bitmap)pictureBox1.Source;

            cbMode.SelectedItem = ObjectDetectorSearchMode.NoOverlap;
            cbScaling.SelectedItem = ObjectDetectorScalingMode.SmallerToGreater;

            HaarCascade cascade =
                HaarCascade.FromXml(
                    Application.GetResourceStream(new Uri(@"Resources/haarcascade_frontalface_alt.xml", UriKind.Relative)).Stream);
            detector = new HaarObjectDetector(cascade, 30);
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            detector.SearchMode = (ObjectDetectorSearchMode)cbMode.SelectedItem;
            detector.ScalingMode = (ObjectDetectorScalingMode)cbScaling.SelectedItem;
            detector.ScalingFactor = 1.5f;
            detector.UseParallelProcessing = false;

            // Process frame to detect objects
            var objects = detector.ProcessFrame(picture);

            if (objects.Length > 0)
            {
                var marker = new RectanglesMarker(objects, Color.FromArgb(0xff, 0xff, 0x00, 0xff));

                // Explicit cast from System.Drawing.Bitmap to ImageSource
                pictureBox1.Source = (ImageSource)marker.Apply(picture);
            }
        }
    }
}