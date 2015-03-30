namespace IPPrototyper.Views
{
    using Windows.UI.Xaml.Controls;

    using IPPrototyper.Core.ViewModels;

    public sealed partial class MainView
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        private void OriginalImagesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                ((MainViewModel)this.ViewModel).ProcessImageCommand.Execute(null);
            }
        }
    }
}
