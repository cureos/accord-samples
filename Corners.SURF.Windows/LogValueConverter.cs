namespace Corners.SURF
{
    using System;

    using Windows.UI.Xaml.Data;

    public class LogValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Math.Pow(10.0, System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}