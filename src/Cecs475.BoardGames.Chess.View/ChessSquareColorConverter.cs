using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.View
{
    class ChessSquareColorConverter : IMultiValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.WhiteSmoke);
        private static SolidColorBrush BLACK_BRUSH = new SolidColorBrush(Colors.Turquoise);
        private static SolidColorBrush DANGER_BRUSH = new SolidColorBrush(Colors.Yellow);
        private static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Colors.LightBlue);
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter will receive two properties: the Position of the square, and whether it
            // is being hovered.
            BoardPosition pos = (BoardPosition)values[0];
            bool isHovered = (bool)values[1];
            bool isSelected = (bool)values[2];
            bool isChecked = (bool)values[3];

            // Hovered squares have a specific color.
            if (isChecked)
                return DANGER_BRUSH;
            else if (isHovered || isSelected)
            {
                return RED_BRUSH;
            }

            else
            {
                if ((pos.Row + pos.Col) % 2 == 0)
                {
                    return WHITE_BRUSH;
                }
                return BLACK_BRUSH;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
