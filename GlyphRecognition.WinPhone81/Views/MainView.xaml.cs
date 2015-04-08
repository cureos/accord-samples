// Copyright (c) 2014-2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace GlyphRecognition.Views
{
    using Windows.UI.Xaml;

    using IPPrototyper.Core.ViewModels;

    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// The first view.
    /// </summary>
    public sealed partial class MainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
        }

        private void MainViewOnLoaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.ViewModel).LoadImagesCommand.Execute(null);
            this.OriginalImagesSelector.Visibility = Visibility.Visible;
        }

        private void OriginalImagesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                ((MainViewModel)this.ViewModel).ProcessImageCommand.Execute(null);
                this.ProcessedImagesSelector.Visibility = Visibility.Visible;
            }
        }
    }
}
