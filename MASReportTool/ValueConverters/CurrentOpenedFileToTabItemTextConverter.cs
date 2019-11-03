using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MASReportTool.ValueConverters
{
    class CurrentOpenedFileToTabItemTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var CurrentOpenedFile = value as string;
            return Path.GetFileNameWithoutExtension(CurrentOpenedFile);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
