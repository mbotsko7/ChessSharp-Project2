
    using Cecs475.BoardGames.View;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Cecs475.BoardGames.Chess.View
    {

        public class ChessSquare : INotifyPropertyChanged
        {
            private int mPlayer;
            public int Player
            {
                get { return mPlayer; }
                set
                {
                    if (value != mPlayer)
                    {
                        mPlayer = value;
                        OnPropertyChanged(nameof(Player));
                    }
                }
            }
            private bool mIsChecked;
            public bool IsChecked
            {
            get { return mIsChecked; }
            set
                {
                if (value != mIsChecked)
                {
                    mIsChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
            }

            private bool mIsHovered;
            public bool IsHovered
            {
                get { return mIsHovered; }
                set
                {
                    if (value != mIsHovered)
                    {
                        mIsHovered = value;
                        OnPropertyChanged(nameof(IsHovered));
                    }
                }
            }

            private bool mIsSelected;
            public bool IsSelected
            {
                get { return mIsSelected; }
                set
                {
                    if (value != mIsSelected)
                    {
                        mIsSelected = value;
                        OnPropertyChanged(nameof(IsSelected));
                    }
                }
            }

           public BoardPosition Position
            {
                get; set;
            }
            private string piece;
            public string Piece
            {
                get
                {
                    return piece;
                }
                set {

                    if (value != piece)
                    {
                        piece = value;
                        OnPropertyChanged(nameof(Piece));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

        }
        public class ChessViewModel : INotifyPropertyChanged, IGameViewModel
        {
            
            private ChessBoard mBoard;
            private ObservableCollection<ChessSquare> mSquares;

            public event PropertyChangedEventHandler PropertyChanged;
            public event EventHandler GameFinished;

            private void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public ChessViewModel()
            {
                mBoard = new ChessBoard();
                mSquares = new ObservableCollection<ChessSquare>(
                from pos in (
                from r in Enumerable.Range(0, 8)
                from c in Enumerable.Range(0, 8)
                select new BoardPosition(r, c)
                )
                select new ChessSquare()
                {
                    Position = pos,
                    Player = mBoard.GetPieceAtPosition(pos).Player,
                    Piece = mBoard.GetPieceAtPosition(pos).ToString()
                    
                    }
                  );
                PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );
                
            }

            

            public ChessSquare hasSelected()
            {
                    foreach(var y in mSquares)
                    {
                        if (y.IsSelected)
                            return y;
                    }
                return null;
            }

            public ObservableCollection<ChessSquare> Squares
            {
                get { return mSquares; }
            }

            public HashSet<ChessMove> PossibleMoves
            {
                get; private set;
            }

            public bool checking(BoardPosition pos)
            {
                if (mBoard.GetPieceAtPosition(pos).PieceType == ChessPieceType.King && mBoard.GetPlayerAtPosition(pos) == mBoard.CurrentPlayer
                    && (mBoard.IsCheck || mBoard.IsCheckmate))
                    return true;
                return false;
            }
            
            public ChessMove history()
            {
                return mBoard.MoveHistory[mBoard.MoveHistory.Count-1] as ChessMove;
            }
            

            public int BoardValue { get { return mBoard.Value; } }

            public int CurrentPlayer { get { return mBoard.CurrentPlayer; } }

            public void UndoMove() 
            {
                ChessMove previous = history();
                if (previous.MoveType == ChessMoveType.PawnPromote)
                    mBoard.UndoLastMove();
                mBoard.UndoLastMove();
                PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );
            var newSquares =
                        from r in Enumerable.Range(0, 8)
                        from c in Enumerable.Range(0, 8)
                        select new BoardPosition(r, c);

            int i = 0;
            foreach (var pos in newSquares)
            {
                mSquares[i].Player = mBoard.GetPieceAtPosition(pos).Player;
                mSquares[i].Piece = mBoard.GetPieceAtPosition(pos).ToString();
                if ((mBoard.IsCheck || mBoard.IsCheckmate) && mBoard.GetPieceAtPosition(pos).PieceType == ChessPieceType.King && mBoard.GetPieceAtPosition(pos).Player == CurrentPlayer)
                    mSquares[i].IsChecked = true;
                else
                    mSquares[i].IsChecked = false;
                i++;
            }
           }
            public bool CanUndo
            {
                get
                {
                    return mBoard.MoveHistory.Count > 0;
                }
            }
            public ChessPiecePosition GetPiece(BoardPosition bp)
            {
                return mBoard.GetPieceAtPosition(bp);
            }

            public void ApplyMove(ChessMove position)
            {
                
                    mBoard.ApplyMove(position);

                    PossibleMoves = new HashSet<ChessMove>(
                        from ChessMove m in mBoard.GetPossibleMoves()
                        select m
                    );
                    var newSquares =
                        from r in Enumerable.Range(0, 8)
                        from c in Enumerable.Range(0, 8)
                        select new BoardPosition(r, c);
                    int i = 0;
                    bool look = false;
                    if (mBoard.IsCheck || mBoard.IsCheckmate)
                        look = true;

                    foreach (var pos in newSquares)
                    {
                        mSquares[i].Player = mBoard.GetPieceAtPosition(pos).Player;
                        mSquares[i].Piece = mBoard.GetPieceAtPosition(pos).ToString();
                        if ((mBoard.IsCheck || mBoard.IsCheckmate)&& mBoard.GetPieceAtPosition(pos).PieceType == ChessPieceType.King && mBoard.GetPieceAtPosition(pos).Player == CurrentPlayer)
                            mSquares[i].IsChecked = true;
                        else
                            mSquares[i].IsChecked = false;
                    
                        i++;
                    }
                
                if (mBoard.IsCheckmate || mBoard.IsStalemate)
                {
                    GameFinished?.Invoke(this, new EventArgs());
                }

            OnPropertyChanged(nameof(BoardValue));
                OnPropertyChanged(nameof(CurrentPlayer));
                OnPropertyChanged(nameof(CanUndo));
                

        
        }

            public void makePromotion()
            {
                var p = new Promotion(this);
                p.ShowDialog();
            }
            public void ApplyMove(BoardPosition position)
            {
                if (mBoard.IsCheckmate || mBoard.IsStalemate)
                {
                    GameFinished?.Invoke(this, new EventArgs());
                }
                else
                {
                    var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
                    foreach (var move in possMoves)
                    {
                        if (move.EndPosition.Equals(position))
                        {
                            mBoard.ApplyMove(move);
                            break;
                        }
                    }

                    PossibleMoves = new HashSet<ChessMove>(
                        from ChessMove m in mBoard.GetPossibleMoves()
                        select m
                    );
                    var newSquares =
                        from r in Enumerable.Range(0, 8)
                        from c in Enumerable.Range(0, 8)
                        select new BoardPosition(r, c);
                    int i = 0;
                    bool look = false;
                    if (mBoard.IsCheck || mBoard.IsCheckmate)
                        look = true;
                    
                    foreach (var pos in newSquares)
                    {
                        mSquares[i].Player = mBoard.GetPieceAtPosition(pos).Player;
                        mSquares[i].Piece = mBoard.GetPieceAtPosition(pos).ToString();
                        
                        i++;
                    }
                }
                
                OnPropertyChanged(nameof(BoardValue));
                OnPropertyChanged(nameof(CurrentPlayer));
                OnPropertyChanged(nameof(CanUndo));
                
            }
        }
    }


