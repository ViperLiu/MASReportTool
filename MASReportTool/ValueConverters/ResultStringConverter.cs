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
            switch (value.ToString().ToLower())
            {
                case "accept":
                    return "符合";
                case "fail":
                    return "不符合";
                case "notfit":
                    return "不適用";
                case "undetermin":
                    return "未檢測";
            }
            return "ERROR";
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
