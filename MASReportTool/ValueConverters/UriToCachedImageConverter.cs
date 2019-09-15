using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MASReportTool.ValueConverters
{
    class UriToCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imgNotFound = "/assets/pics/icon_image_not_found.png";

            if (value == null)
                return imgNotFound;

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                try
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(value.ToString());
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    return bi;
                }
                catch(Exception e)
                {
                    Console.WriteLine("[WARNING] " + e.Message);
                    return imgNotFound;
                }
            }

            return imgNotFound;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }
}
