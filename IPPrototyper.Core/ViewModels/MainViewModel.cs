using Cirrious.MvvmCross.ViewModels;

namespace IPPrototyper.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Windows.Input;

    using AForge.Imaging.IPPrototyper;

    using IPPrototyper.Core.Services;

    public class MainViewModel 
        : MvxViewModel
    {
        #region FIELDS

        private readonly IImageLoaderService imageLoaderService;

        private readonly IImageProcessingRoutine imageProcessingRoutine;

        private readonly ImageProcessingLog imageProcessingLog;

        private ObservableCollection<Bitmap> originalImages;

        private ObservableCollection<KeyValuePair<string, Bitmap>> processedImages;

        private Bitmap selectedOriginalImage;

        private KeyValuePair<string, Bitmap> selectedProcessedImage;

        private Bitmap displayedImage;

        private string log;

        private ICommand loadImagesCommand;

        private ICommand processImageCommand;

        #endregion

        #region CONSTRUCTORS

        public MainViewModel(IImageLoaderService imageLoaderService, IImageProcessingService imageProcessingService)
        {
            this.imageLoaderService = imageLoaderService;
            this.imageProcessingRoutine = imageProcessingService.Current;

            this.imageProcessingLog = new ImageProcessingLog();
        }

        #endregion

        #region PROPERTIES

        public string RoutineName
        {
            get
            {
                return this.imageProcessingRoutine.Name;
            }
        }

        public ObservableCollection<Bitmap> OriginalImages
        {
            get
            {
                return this.originalImages;
            }
            private set
            {
                this.originalImages = value;
                this.RaisePropertyChanged(() => this.OriginalImages);
            }
        }

        public ObservableCollection<KeyValuePair<string, Bitmap>> ProcessedImages
        {
            get
            {
                return this.processedImages;
            }
            private set
            {
                this.processedImages = value;
                this.RaisePropertyChanged(() => this.ProcessedImages);
            }
        }

        public Bitmap SelectedOriginalImage
        {
            get
            {
                return this.selectedOriginalImage;
            }
            set
            {
                this.selectedOriginalImage = value;
                this.RaisePropertyChanged(() => this.SelectedOriginalImage);
                if (this.selectedOriginalImage != null)
                {
                    this.DisplayedImage = this.selectedOriginalImage;
                }
            }
        }

        public KeyValuePair<string, Bitmap> SelectedProcessedImage
        {
            get
            {
                return this.selectedProcessedImage;
            }
            set
            {
                this.selectedProcessedImage = value;
                this.RaisePropertyChanged(() => this.SelectedProcessedImage);
                if (this.selectedProcessedImage.Value != null)
                {
                    this.DisplayedImage = this.selectedProcessedImage.Value;
                }
            }
        }

        public Bitmap DisplayedImage
        {
            get
            {
                return this.displayedImage;
            }
            set
            {
                this.displayedImage = value;
                this.RaisePropertyChanged(() => this.DisplayedImage);
            }
        }

        public string Log
        {
            get
            {
                return this.log;
            }
            private set
            {
                this.log = value;
                this.RaisePropertyChanged(() => this.Log);
            }
        }

        public ICommand LoadImagesCommand
        {
            get
            {
                return this.loadImagesCommand ?? (this.loadImagesCommand = new MvxCommand(this.LoadImagesAsync));
            }
        }

        public ICommand ProcessImageCommand
        {
            get
            {
                return this.processImageCommand
                       ?? (this.processImageCommand =
                           new MvxCommand(this.ProcessImage, () => this.SelectedOriginalImage != null));
            }
        }

        #endregion

        #region METHODS

        public async void LoadImagesAsync()
        {
            var images = await this.imageLoaderService.LoadImagesAsync();
            if (images != null)
            {
                this.OriginalImages = new ObservableCollection<Bitmap>(images);
            }
        }

        private void ProcessImage()
        {
            if (this.DisplayedImage == null)
            {
                return;
            }

            this.imageProcessingLog.Clear();
            this.imageProcessingLog.AddImage("Source", this.DisplayedImage);
            this.imageProcessingRoutine.Process(this.DisplayedImage, this.imageProcessingLog);

            this.ProcessedImages = new ObservableCollection<KeyValuePair<string, Bitmap>>(
                this.imageProcessingLog.Images);

            this.Log = String.Join(Environment.NewLine, this.imageProcessingLog.Messages);
        }

        #endregion
    }
}
