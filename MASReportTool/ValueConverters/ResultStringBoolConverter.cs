using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MASReportTool.ValueConverters
{
    class ResultStringBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ITestResult result;
            try
            {
                result = (ITestResult)value;
                if (result.GetDisplayString() == "符合" && (string)parameter == "accept")
                    return true;
                else if (result.GetDisplayString() == "不符合" && (string)parameter == "fail")
                    return true;
                else return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
