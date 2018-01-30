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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Cecs475.BoardGames.Chess.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChessView : UserControl
    {
        public static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.Green);
        public ChessView()
        {
            InitializeComponent();
        }
        public ChessViewModel Model
        {
            get { return FindResource("vm") as ChessViewModel; }
        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            var cs = vm.hasSelected();
            if (cs == null)
                return;
            
            else
            {
                BoardPosition bp = cs.Position;
                ChessMove cm = new ChessMove(bp, square.Position);
                foreach (var x in vm.PossibleMoves)
                {
                    if (cm.Equals(x))
                        square.IsHovered = true;
                }

            }
            
        }


        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHovered = false;
        }
        //private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Border b = sender as Border;
        //    var square = b.DataContext as ChessSquare;
        //    var vm = FindResource("vm") as ChessViewModel;
        //    //Console.WriteLine($"Current player{vm.CurrentPlayer} square player{square.Player}");
        //    ChessSquare cs = vm.hasSelected();
        //    Console.WriteLine("!!!!!!!!!!");
        //    foreach(var x in vm.PossibleMoves)
        //        Console.WriteLine(x.ToString());
        //    if (vm.checking(square.Position))
        //        square.IsChecked = true;
        //    else if (cs == null) //if not selected, selected
        //        square.IsSelected = true;
            
        //    else //if (vm.PossibleMoves.Contains(new ChessMove(square.Position, bp)))
        //    {
        //        BoardPosition bp = cs.Position;
        //        ChessMove cm = new ChessMove(bp, square.Position);
        //        foreach (var x in vm.PossibleMoves)
        //        {
        //            if (bp.Equals(x.StartPosition) && square.Position.Equals(x.EndPosition))
        //            {
                        
        //                vm.ApplyMove(new ChessMove(bp, square.Position));
        //                Console.WriteLine("!!!!!!!!!!");
        //                foreach (var xxx in vm.PossibleMoves)
        //                    Console.WriteLine(xxx.ToString());
        //                if ((square.Position.Row == 0 && vm.GetPiece(square.Position).PieceType == ChessPieceType.Pawn && vm.GetPiece(square.Position).Player == 1)
        //                    || (square.Position.Row == 7 && vm.GetPiece(square.Position).PieceType == ChessPieceType.Pawn && vm.GetPiece(square.Position).Player == 2))
        //                {
        //                    //vm.makePromotion();
        //                    if(vm.GetPiece(square.Position).Player == 1)
        //                    {
        //                        var promotion = new PromotionWhite(vm);
        //                        promotion.ShowDialog();
        //                    }
        //                    else
        //                    {
        //                         var promotion = new Promotion(vm);
        //                                                    //promotion.Closed += promotion_closed;
        //                        promotion.ShowDialog();
        //                    }
                           

        //                    //this.Hide();
        //                }
        //                square.IsHovered = false;
        //                cs.IsSelected = false;
        //                if (vm.checking(square.Position))
        //                    square.IsChecked = true;
        //                return;
        //            }
        //        }
        //        cs.IsSelected = false;
                
        //    }
        //    //else if (vm.PossibleMoves.Contains(new ChessMove(square.Position, bp)) == false)
        //    //square.IsSelected = false;
        //    if (vm.checking(square.Position))
        //        square.IsChecked = true;
        //}

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            ChessSquare cs = vm.hasSelected();
            if (cs == null) //if not selected, selected
                square.IsSelected = true;

            else //if (vm.PossibleMoves.Contains(new ChessMove(square.Position, bp)))
            {
                BoardPosition bp = cs.Position;
                ChessMove cm = new ChessMove(bp, square.Position);
                foreach (var x in vm.PossibleMoves)
                {
                    if (bp.Equals(x.StartPosition) && square.Position.Equals(x.EndPosition))
                    {

                        vm.ApplyMove(new ChessMove(bp, square.Position, x.MoveType));
                        
                        if ((square.Position.Row == 0 && vm.GetPiece(square.Position).PieceType == ChessPieceType.Pawn && vm.GetPiece(square.Position).Player == 1)
                            || (square.Position.Row == 7 && vm.GetPiece(square.Position).PieceType == ChessPieceType.Pawn && vm.GetPiece(square.Position).Player == 2))
                        {
                            //vm.makePromotion();
                            if (vm.GetPiece(square.Position).Player == 1)
                            {
                                var promotion = new PromotionWhite(vm);
                                promotion.ShowDialog();
                            }
                            else
                            {
                                var promotion = new Promotion(vm);
                                //promotion.Closed += promotion_closed;
                                promotion.ShowDialog();
                            }


                            //this.Hide();
                        }
                        square.IsHovered = false;
                        cs.IsSelected = false;
                        //if (vm.checking(square.Position))
                        //    square.IsChecked = true;
                        return;
                    }
                }
                cs.IsSelected = false;

            }


        }


    }

}
