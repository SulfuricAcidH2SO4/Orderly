using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Orderly.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not string) return null;
            try {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString()));
            }
            catch {
                return null!;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
