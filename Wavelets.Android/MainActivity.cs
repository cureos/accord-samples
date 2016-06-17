// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainActivity.cs" company="Cureos AB">
//   Copyright (c) 2015 Anders Gustafsson, Cureos AB
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Wavelets
{
    using System;

    using Accord.Imaging.Filters;
    using Accord.Math.Wavelets;

    using Android.App;
    using Android.Graphics;
    using Android.OS;
    using Android.Widget;

    using PixelFormat = System.Drawing.Imaging.PixelFormat;

    /// <summary>
    /// </summary>
    [Activity(Label = "Wavelets.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IWavelet wavelet;

        private System.Drawing.Bitmap lenna;

        private System.Drawing.Bitmap transformed;

        private ImageView transformImage;

        private Spinner transformsSpinner;

        private Spinner iterationsSpinner;

        /// <summary>
        /// </summary>
        /// <param name="bundle">
        /// </param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            // Load image
            var filters = new FiltersSequence(Grayscale.CommonAlgorithms.BT709, new ResizeBicubic(512, 512));
            var tmp = (System.Drawing.Bitmap)BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.lena512);
            this.lenna = filters.Apply(tmp.Clone(PixelFormat.Format32bppArgb));

            this.transformImage = this.FindViewById<ImageView>(Resource.Id.TransformImage);
            this.transformImage.SetImageBitmap((Android.Graphics.Bitmap)this.lenna);

            // Populate the transforms spinner
            this.transformsSpinner = this.FindViewById<Spinner>(Resource.Id.TransformSpinner);

            var transformsAdapter = ArrayAdapter.CreateFromResource(
                this,
                Resource.Array.Transforms,
                Android.Resource.Layout.SimpleSpinnerItem);
            transformsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.transformsSpinner.Adapter = transformsAdapter;

            // Configure the iterations spinner
            this.iterationsSpinner = this.FindViewById<Spinner>(Resource.Id.IterationsSpinner);

            var iterationsAdapter = ArrayAdapter.CreateFromResource(
                this,
                Resource.Array.Iterations,
                Android.Resource.Layout.SimpleSpinnerItem);
            iterationsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.iterationsSpinner.Adapter = iterationsAdapter;

            // Define click handlers
            var forwardButton = this.FindViewById<Button>(Resource.Id.ForwardButton);
            forwardButton.Click += this.ForwardButtonOnClick;

            var backwardButton = this.FindViewById<Button>(Resource.Id.BackwardButton);
            backwardButton.Click += this.BackwardButtonOnClick;

            var originalButton = this.FindViewById<Button>(Resource.Id.OriginalButton);
            originalButton.Click += this.OriginalButtonOnClick;
        }

        private void ForwardButtonOnClick(object sender, System.EventArgs e)
        {
            var selectedTransform = (string)this.transformsSpinner.SelectedItem;
            var numIterations = Convert.ToInt32((string)this.iterationsSpinner.SelectedItem);
            if (selectedTransform == "Haar")
            {
                this.wavelet = new Haar(numIterations);
            }
            else
            {
                this.wavelet = new CDF97(numIterations);
            }

            // Create forward transform
            var wt = new WaveletTransform(this.wavelet);

            // Apply forward transform
            this.transformed = wt.Apply(this.lenna);

            this.transformImage.SetImageBitmap((Android.Graphics.Bitmap)this.transformed);
        }

        private void BackwardButtonOnClick(object sender, EventArgs eventArgs)
        {
            // Create inverse transform
            var wt = new WaveletTransform(this.wavelet, true);

            // Apply inverse transform
            this.transformImage.SetImageBitmap((Android.Graphics.Bitmap)wt.Apply(this.transformed));
        }

        private void OriginalButtonOnClick(object sender, EventArgs eventArgs)
        {
            this.transformImage.SetImageBitmap((Android.Graphics.Bitmap)this.lenna);
        }
    }
}

