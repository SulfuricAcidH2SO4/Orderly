using Orderly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Orderly.Converters
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            SortingOption option = Enum.Parse<SortingOption>(value.ToString());
            return (int)option;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            if (!int.TryParse(value.ToString(), out int intEnum)) return null;
            return (SortingOption)Enum.ToObject(typeof(SortingOption), intEnum);
        }
    }
}
