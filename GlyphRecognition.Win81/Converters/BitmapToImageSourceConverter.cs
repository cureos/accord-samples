namespace GlyphRecognition.Converters
{
    using System;
    using System.Drawing;

    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media.Imaging;

    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Bitmap)) return null;
            return (WriteableBitmap)(Bitmap)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is WriteableBitmap)) return null;
            return (Bitmap)(WriteableBitmap)value;
        }
    }
}