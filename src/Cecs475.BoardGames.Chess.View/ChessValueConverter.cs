using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cecs475.BoardGames.Chess.View
{
    class ChessValueConverter : IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                int v = (int)value;
                if (v == 0)
                    return "Tie game";
                if (v < 0)
                    return $"Black has a +{-v} advantage";
                return $"White has a +{v} advantage";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

    }
}
