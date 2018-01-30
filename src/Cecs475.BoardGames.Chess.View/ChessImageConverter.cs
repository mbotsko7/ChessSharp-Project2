using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cecs475.BoardGames.Chess.View
{
    class ChessImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string mPiece = value as string;
                if (mPiece != null)
                {
                    Image img = new Image()
                    {
                        Source = new BitmapImage(new Uri("./resources/" + mPiece + ".png", UriKind.Relative))};
                    return img;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
