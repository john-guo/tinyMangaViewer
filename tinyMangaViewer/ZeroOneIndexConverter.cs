using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace tinyMangaViewer
{
    public enum ZeroOneIndexConverterEnum
    {
        ZeroToOne,
        OneToZero
    }

    public class ZeroOneIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int n)
            {
                var type = ZeroOneIndexConverterEnum.ZeroToOne;
                if (parameter != null)
                    type = (ZeroOneIndexConverterEnum)parameter;
                return  type == ZeroOneIndexConverterEnum.ZeroToOne ? n + 1 : n - 1;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
