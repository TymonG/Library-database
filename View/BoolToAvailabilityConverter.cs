using System.Globalization;
using System.Windows.Data;

namespace View;

[ValueConversion(typeof(bool), typeof(string))]
public class BoolToAvailabilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? "Available" : "Borrowed";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
