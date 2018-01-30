using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.View
{
    /// <summary>
    /// Interaction logic for Promotion.xaml
    /// </summary>
    public partial class PromotionWhite : Window
    {
        private ChessViewModel vm;
        private ChessMove hist;
        private bool hold;
        public PromotionWhite(ChessViewModel v)
        {
            InitializeComponent();
            
            
            vm = v;
            hist = vm.history();
            hold = false;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Button b = (Button)sender;
            ChessMove cm;
            if (b.Tag.Equals("k"))
            {
                cm = new ChessMove(hist.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Knight), ChessMoveType.PawnPromote);
            }
            else if (b.Tag.Equals("b"))
            {
                cm = new ChessMove(hist.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Bishop), ChessMoveType.PawnPromote);
            }
            else if (b.Tag.Equals("r"))
            {
                cm = new ChessMove(hist.EndPosition, new BoardPosition(-1, (int)ChessPieceType.RookPawn), ChessMoveType.PawnPromote);
            }
            else //if (b.Tag.Equals("q"))
            {
                cm = new ChessMove(hist.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Queen), ChessMoveType.PawnPromote);
            }
            vm.ApplyMove(cm);

            this.Close();
        }

    }
}
