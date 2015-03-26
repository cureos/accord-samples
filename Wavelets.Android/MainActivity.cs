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

    /// <summary>
    /// </summary>
    [Activity(Label = "Wavelets.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IWavelet wavelet;

        private Bitmap lenna;

        private Bitmap transformed;

        private ImageView transformImage;

        private NumberPicker iterationsPicker;

        private Spinner transformsSpinner;

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
            this.lenna = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.lena512);

            this.transformImage = this.FindViewById<ImageView>(Resource.Id.TransformImage);
            this.transformImage.SetImageBitmap(this.lenna);

            // Populate the transforms spinner
            this.transformsSpinner = this.FindViewById<Spinner>(Resource.Id.TransformSpinner);

            var transformsAdapter = ArrayAdapter.CreateFromResource(
                this,
                Resource.Array.Transforms,
                Android.Resource.Layout.SimpleSpinnerItem);
            transformsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.transformsSpinner.Adapter = transformsAdapter;

            // Configure the iterations picker
            this.iterationsPicker = this.FindViewById<NumberPicker>(Resource.Id.IterationsPicker);
            this.iterationsPicker.MinValue = 1;
            this.iterationsPicker.MaxValue = 5;

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
            var numIterations = this.iterationsPicker.Value;
            if (selectedTransform == "Haar")
            {
                this.wavelet = new Haar(numIterations);
            }
            else
            {
                this.wavelet = new CDF97((int)numIterations);
            }

            // Create forward transform
            var wt = new WaveletTransform(this.wavelet);

            // Apply forward transform
            this.transformed = (Android.Graphics.Bitmap)wt.Apply((System.Drawing.Bitmap)this.lenna);

            this.transformImage.SetImageBitmap(this.transformed);
        }

        private void BackwardButtonOnClick(object sender, EventArgs eventArgs)
        {
            // Create inverse transform
            var wt = new WaveletTransform(this.wavelet, true);

            // Apply inverse transform
            this.transformImage.SetImageBitmap((Android.Graphics.Bitmap)wt.Apply((System.Drawing.Bitmap)this.transformed));
        }

        private void OriginalButtonOnClick(object sender, EventArgs eventArgs)
        {
            this.transformImage.SetImageBitmap(this.lenna);
        }
    }
}

