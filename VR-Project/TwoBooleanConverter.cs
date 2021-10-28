using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace VR_Project
{
    class TwoBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool b1 = values[0].GetType() == typeof(bool) ? (bool) values[0] : false ;
            bool b2 = values[1].GetType() == typeof(bool) ? (bool)values[1] : false;

            return !b1 && b2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
