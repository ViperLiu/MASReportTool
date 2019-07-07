using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MASReportTool.ValueConverters
{
    class ResultStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ITestResult).GetDisplayString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "符合":
                    return "accept";
                case "不符合":
                    return "fail";
                case "不適用":
                    return "notfit";
                case "未檢測":
                    return "undetermin";
            }
            return "ERROR";
        }
    }
}
