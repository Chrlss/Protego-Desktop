using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Protego.Class
{
    class TempToText
    {
        public class TemperatureToTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double temperature && parameter is string param)
                {
                    if (param.Equals("Temperature"))
                    {
                        return $"{temperature:F1} °C"; // Format temperature with 1 decimal place
                    }
                }
                return value.ToString(); // Return the original value if conversion not applicable
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException(); // We don't need ConvertBack for this scenario
            }
        }
    }
}
