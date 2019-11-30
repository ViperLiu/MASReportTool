using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MASReportTool.ValueConverters
{
    class ResultStringIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconPath = (value as ITestResult).GetIconPath();
            ImageSource imgSource = new BitmapImage();
            if (iconPath == "")
            {
                return imgSource;
            }
            Uri uri = new Uri(iconPath, UriKind.Relative);
            imgSource = new BitmapImage(uri);
            return imgSource;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
