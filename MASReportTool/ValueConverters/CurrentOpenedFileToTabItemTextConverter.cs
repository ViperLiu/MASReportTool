using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MASReportTool.ValueConverters
{
    class CurrentOpenedFileToTabItemTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isSaved = (bool)values[0];
            var currentOpenedFile = values[1] as string;
            var titleText = Path.GetFileNameWithoutExtension(currentOpenedFile);
            titleText = titleText.Replace("*", "");
            if (!isSaved)
                titleText = "*" + titleText;
            return titleText;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
