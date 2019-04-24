using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MASReportTool
{
    class VisibilityBoolConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value.ToString().ToLower())
            {
                case "Hidden":
                    return false;
                case "Visible":
                    return true;
            }
            return false;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return "Visible";
            }
                
            else if ((bool)value == false)
                return "Hidden";
            else
                return "Hidden";
        }
    }
}
