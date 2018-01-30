using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess
{

    public class ChessBoard : IGameBoard
    {
        /// <summary>
        /// The number of rows and columns on the chess board.
        /// </summary>
        public const int BOARD_SIZE = 8;

        // Reminder: there are 3 different types of rooks
        private sbyte[,] mBoard = new sbyte[8, 8] {
            {-2, -4, -5, -6, -7, -5, -4, -3 },
            {-1, -1, -1, -1, -1, -1, -1, -1 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {1, 1, 1, 1, 1, 1, 1, 1 },
            {2, 4, 5, 6, 7, 5, 4, 3 }
        };

        private List<BoardPosition> moved = new List<BoardPosition>();
        private BoardPosition pawnMovedTwo = new BoardPosition(-1, -1);
        private bool first, promotion;
        // TODO:
        // You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
        // if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
        // happened. There are several ways to do it and I leave it up to you.


        /// <summary>
        /// Constructs a new chess board with the default starting arrangement.
        /// </summary>
        public ChessBoard()
        {
            MoveHistory = new List<IGameMove>();
            first = true;
            Value = 0;
            promotion = false;
            // TODO:
            // Finish any other one-time setup.
        }

        /// <summary>
        /// Constructs a new chess board by only placing pieces as specified.
        /// </summary>
        /// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
        /// position of a particular piece to place on the board</param>
        public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions)

            : this()
        { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST


            foreach (int i in Enumerable.Range(0, 8))
            { // another way of doing for i = 0 to < 8
                foreach (int j in Enumerable.Range(0, 8))
                {
                    mBoard[i, j] = 0;
                }
            }
            foreach (var pos in startingPositions)
            {
                SetPosition(pos.Item1, pos.Item2);
            }
        }

        /// <summary>
        /// A difference in piece values for the pieces still controlled by white vs. black, where
        /// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
        /// </summary>
        public int Value { get; private set; }

        public bool kingandcheck(BoardPosition bp) {
            if (GetPieceAtPosition(bp).PieceType == ChessPieceType.King && (IsCheckmate || IsCheckmate))
                return true;
            return false;
        }

        public int CurrentPlayer
        {
            get
            {
                return first == true ? 1 : 2;
            }
        }

        // An auto-property suffices here.
        public IList<IGameMove> MoveHistory
        {
            get; private set;
        }

        /// <summary>
        /// Returns the piece and player at the given position on the board.
        /// </summary>
        public ChessPiecePosition GetPieceAtPosition(BoardPosition position)
        {
            var boardVal = mBoard[position.Row, position.Col];
            return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]),
                boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
        }



        public void ApplyMove(IGameMove move)
        {

            promotion = false;
            ChessMove m = move as ChessMove;
            if (m.MoveType == ChessMoveType.PawnPromote)
            {
                int player = CurrentPlayer == 1 ? 1 : -1;
                m.Piece = GetPieceAtPosition(m.StartPosition);
                mBoard[m.StartPosition.Row, m.StartPosition.Col] = (sbyte)(m.EndPosition.Col * player);
                if (GetPlayerAtPosition(m.StartPosition) == 1)
                    Value += GetPieceValue(GetPieceAtPosition(m.StartPosition).PieceType);
                else
                    Value -= GetPieceValue(GetPieceAtPosition(m.StartPosition).PieceType);
                Value += player * -1;
                //Console.WriteLine("ayy");
            }
            else if (m.MoveType == ChessMoveType.Normal && PositionIsEnemy(m.EndPosition, CurrentPlayer))
            {
                m.Piece = GetPieceAtPosition(m.StartPosition);
                m.Captured = GetPieceAtPosition(m.EndPosition);
                if (m.Captured.Player == 1)
                    Value -= GetPieceValue(m.Captured.PieceType);
                else
                    Value += GetPieceValue(m.Captured.PieceType);
                SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                if (GetPieceAtPosition(m.EndPosition).PieceType == ChessPieceType.Pawn
                    && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7))
                {
                    promotion = true;
                    first = !first;

                }
            }
            else if (m.MoveType == ChessMoveType.Normal && PositionIsEmpty(m.EndPosition))
            {
                m.Piece = GetPieceAtPosition(m.StartPosition);
                SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                if (GetPieceAtPosition(m.EndPosition).PieceType == ChessPieceType.Pawn
                    && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7))
                {
                    promotion = true;
                    first = !first;

                }

            }

            else if (m.MoveType == ChessMoveType.CastleKingSide)
            {
                m.Piece = GetPieceAtPosition(m.StartPosition);
                SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                BoardPosition rook = new BoardPosition(CurrentPlayer == 1 ? 7 : 0, 7);
                SetPosition(new BoardPosition(m.StartPosition.Row, m.EndPosition.Col - 1), GetPieceAtPosition(rook));
                SetPosition(rook, new ChessPiecePosition(ChessPieceType.Empty, 0));
            }
            else if (m.MoveType == ChessMoveType.CastleQueenSide)
            {
                m.Piece = GetPieceAtPosition(m.StartPosition);
                SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                BoardPosition rook = new BoardPosition(CurrentPlayer == 1 ? 7 : 0, 0);
                SetPosition(new BoardPosition(m.StartPosition.Row, m.EndPosition.Col + 1), GetPieceAtPosition(rook));
                SetPosition(rook, new ChessPiecePosition(ChessPieceType.Empty, 0));
            }
            else if (m.MoveType == ChessMoveType.EnPassant)
            {
                m.Piece = GetPieceAtPosition(m.StartPosition);
                SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
                SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                BoardPosition passing = new BoardPosition(m.EndPosition.Row + (first ? 1 : -1), m.EndPosition.Col);
                m.Captured = GetPieceAtPosition(passing);
                SetPosition(passing, new ChessPiecePosition(ChessPieceType.Empty, 0));
                if (m.Captured.Player == 1)
                    Value -= GetPieceValue(m.Captured.PieceType);
                else
                    Value += GetPieceValue(m.Captured.PieceType);
            }

            first = !first;
            // Console.WriteLine($"MoveHistory is {MoveHistory.Count}");
            MoveHistory.Add(m);
            // Console.WriteLine($"Post MoveHistory is {MoveHistory.Count}");



        }
        public void UndoLastMove()
        {
            // TODO: implement this method. Make sure to account for "special" moves.
            //jjj
            if (MoveHistory.Count == 0)
            {
                throw new InvalidOperationException("No moves to undo.\n");
            }
            ChessMove m = MoveHistory[MoveHistory.Count - 1] as ChessMove;
            //change the player
            // Console.WriteLine($"Undoing move {m.ToString()}");

            if (promotion)
            {
                promotion = false;
                first = !first;
            }
            int player = first ? 1 : -1;
            if (m.MoveType == ChessMoveType.Normal)
            {
                //first move the position that moved
                SetPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));
                //then restore a piece if Captured
                if (!m.Captured.Equals(null))
                {
                    SetPosition(m.EndPosition, m.Captured);
                    if (m.Captured.Player == 1)
                        Value += GetPieceValue(m.Captured.PieceType);
                    else
                        Value -= GetPieceValue(m.Captured.PieceType);
                }
            }
            else if (m.MoveType == ChessMoveType.EnPassant)
            {
                SetPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));
                SetPosition((MoveHistory[MoveHistory.Count - 2] as ChessMove).EndPosition, m.Captured);
                SetPosition(m.EndPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                if (m.Captured.Player == 1)
                    Value += GetPieceValue(m.Captured.PieceType);
                else
                    Value -= GetPieceValue(m.Captured.PieceType);
                //jfsaljfd
            }
            else if (m.MoveType == ChessMoveType.CastleKingSide)
            {
                SetPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));
                SetPosition(m.EndPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                BoardPosition rook = new BoardPosition(first ? 0 : 7, 5);
                SetPosition(new BoardPosition(first ? 0 : 7, 7), GetPieceAtPosition(rook));
                //SetPosition(rook, GetPieceAtPosition(new BoardPosition(first ? 0 : 7, 7)));
                SetPosition(rook, new ChessPiecePosition(ChessPieceType.Empty, 0));
            }
            else if (m.MoveType == ChessMoveType.CastleQueenSide)
            {
                SetPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));
                SetPosition(m.EndPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                BoardPosition rook = new BoardPosition(first ? 0 : 7, 3);
                SetPosition(new BoardPosition(first ? 0 : 7, 0), GetPieceAtPosition(rook));
                //SetPosition(rook, GetPieceAtPosition(new BoardPosition(first ? 0 : 7, 7)));
                SetPosition(rook, new ChessPiecePosition(ChessPieceType.Empty, 0));

            }
            else if (m.MoveType == ChessMoveType.PawnPromote)
            {
                first = !first;
                int p = CurrentPlayer == 1 ? 1 : -1;
                if (GetPlayerAtPosition(m.StartPosition) == 1)
                {
                    Value -= GetPieceValue(GetPieceAtPosition(m.StartPosition).PieceType);
                }
                else
                {
                    Value += GetPieceValue(GetPieceAtPosition(m.StartPosition).PieceType);
                }

                Value += p;
                int positive = mBoard[m.StartPosition.Row, m.StartPosition.Col] > 0 ? 1 : -1;
                mBoard[m.StartPosition.Row, m.StartPosition.Col] = (sbyte)(1 * positive);
                //first = !first;
                promotion = true;
                MoveHistory.RemoveAt(MoveHistory.Count - 1);
                return;
            }
            else
            {
                SetPosition(m.EndPosition, m.Captured);
                // ChessMove m2 = MoveHistory[MoveHistory.Count-2] as ChessMove;
                // SetPosition(m2.StartPosition, m2.Piece);
                // mBoard[m2.EndPosition.Row, m2.EndPosition.Col];
            }
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
            first = !first;

        }

        public IEnumerable<IGameMove> GetPromotionMoves()
        {
            List<ChessMove> list = new List<ChessMove>();
            int row = -1, col = -1;
            for (int r = 0; r < 8; r += 7)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (mBoard[r, c] == 1 || mBoard[r, c] == -1)
                    {
                        row = r;
                        col = c;
                    }
                }
            }
            BoardPosition position = new BoardPosition(row, col);
            BoardPosition rook = new BoardPosition(-1, (int)ChessPieceType.RookPawn);
            BoardPosition bishop = new BoardPosition(-1, (int)ChessPieceType.Bishop);
            BoardPosition knight = new BoardPosition(-1, (int)ChessPieceType.Knight);
            BoardPosition queen = new BoardPosition(-1, (int)ChessPieceType.Queen);

            list.Add(new ChessMove(position, rook, ChessMoveType.PawnPromote));
            list.Add(new ChessMove(position, bishop, ChessMoveType.PawnPromote));
            list.Add(new ChessMove(position, queen, ChessMoveType.PawnPromote));
            list.Add(new ChessMove(position, knight, ChessMoveType.PawnPromote));
            return list;
        }
        private void printHistory()
        {
            Console.WriteLine("Printing History!!!!!!!!!!");
            foreach (ChessMove m in MoveHistory)
            {
                Console.WriteLine(m.ToString());
            }
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~");
        }
        public IEnumerable<IGameMove> GetPossibleMoves()
        {
            // printHistory();
            if (promotion)
            {
                return GetPromotionMoves();
            }
            List<BoardPosition> positions = GetPlayerPieces(CurrentPlayer);
            List<ChessMove> possible = new List<ChessMove>();
            foreach (BoardPosition position in positions)
            {
                ChessPiecePosition cpos = GetPieceAtPosition(position);
                if (cpos.PieceType == ChessPieceType.Pawn)
                {
                    List<ChessMove> temp = PawnPossible(position);//convertMoveList(position, PawnPossible(position));
                    possible.AddRange(temp);

                }
                else if (cpos.PieceType == ChessPieceType.Knight)
                {
                    possible.AddRange(KnightPossible(position));//convertMoveList(position, KnightPossible(position)));
                }
                else if (cpos.PieceType == ChessPieceType.Bishop)
                {
                    possible.AddRange(BishopPossible(position));//convertMoveList(position, BishopPossible(position)));
                }
                else if (cpos.PieceType == ChessPieceType.RookKing ||
                    cpos.PieceType == ChessPieceType.RookPawn ||
                    cpos.PieceType == ChessPieceType.RookQueen)
                {
                    possible.AddRange(RookPossible(position));//convertMoveList(position, RookPossible(position)));
                }
                else if (cpos.PieceType == ChessPieceType.Queen)
                {
                    possible.AddRange(QueenPossible(position));//convertMoveList(position, QueenPossible(position)));
                }
                else
                    possible.AddRange(KingPossible(position));//convertMoveList(position, KingPossible(position)));
            }
            //Console.WriteLine($"board preval is {Value}");
            possible = EndangerKing(possible, findKing(CurrentPlayer));
            //Console.WriteLine($"Board postvalue {Value}");
            return possible;

        }


        private BoardPosition findKing(int player)
        {
            int king = player == 1 ? 7 : -7;

            int kRow = -1, kCol = -1;
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (mBoard[r, c] == king)
                    {
                        kRow = r;
                        kCol = c;

                    }
                }
            }
            return new BoardPosition(kRow, kCol);
        }

        private List<ChessMove> EndangerKing(List<ChessMove> m, BoardPosition kp)
        {
            //why not
            //w
            for (int i = 0; i < m.Count; i++)
            {
                //Console.WriteLine($"Before...The move is {m[i].ToString()} and the value is {Value} and type of {m[i].MoveType}");
                bool before = first;
                ApplyMove(m[i]);
                if (before == first)
                {
                    var mythreat = GetThreatenedPositions(first ? 2 : 1) as List<BoardPosition>;
                    if (mythreat.Contains(findKing(CurrentPlayer)))
                    {
                        m.RemoveAt(i);
                        i--;
                        UndoLastMove();
                        continue;
                    }
                }
                var threat = GetThreatenedPositions(CurrentPlayer) as List<BoardPosition>;

                foreach (BoardPosition move in threat)
                {

                    if (move.Equals(findKing(first ? 2 : 1)))
                    {
                        m.RemoveAt(i);
                        i--;
                        break;
                    }

                }
                UndoLastMove();

            }
            return m;

        }

        private List<BoardPosition> convertMoveList(List<ChessMove> l, int byPlayer)
        {
            List<BoardPosition> ret = new List<BoardPosition>();
            int p = byPlayer == 1 ? -1 : 1;
            foreach (ChessMove pos in l)
            {
                if (pos.MoveType == ChessMoveType.EnPassant)
                    ret.Add(new BoardPosition(pos.EndPosition.Row + p, pos.EndPosition.Col));
                else
                    ret.Add(pos.EndPosition);
            }
            return ret;
        }
        /// <summary>
        /// Gets a sequence of all positions on the board that are threatened by the given player. A king
        /// may not move to a square threatened by the opponent.
        /// </summary>
        public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer)
        {
            // TODO: implement this method. Make sure to account for "special" moves.
            List<BoardPosition> positions = GetPlayerPieces(byPlayer);
            List<ChessMove> threatened = new List<ChessMove>();
            foreach (BoardPosition position in positions)
            {
                ChessPiecePosition cpos = GetPieceAtPosition(position);
                if (cpos.PieceType == ChessPieceType.Pawn)
                    threatened.AddRange(PawnThreat(position));
                else if (cpos.PieceType == ChessPieceType.Knight)
                {
                    threatened.AddRange(KnightThreat(position));
                }
                else if (cpos.PieceType == ChessPieceType.Bishop)
                {
                    threatened.AddRange(BishopThreat(position));
                }
                else if (cpos.PieceType == ChessPieceType.RookKing ||
                    cpos.PieceType == ChessPieceType.RookPawn ||
                    cpos.PieceType == ChessPieceType.RookQueen)
                {
                    threatened.AddRange(RookThreat(position));
                }
                else if (cpos.PieceType == ChessPieceType.Queen)
                {
                    threatened.AddRange(QueenThreat(position));
                }
                else
                    threatened.AddRange(KingThreat(position));
            }
            return convertMoveList(threatened, byPlayer);
        }
        private bool check()
        {
            BoardPosition k = findKing(CurrentPlayer);
            List<BoardPosition> threat = GetThreatenedPositions(CurrentPlayer == 1 ? 2 : 1) as List<BoardPosition>;
            var x = GetPossibleMoves() as List<ChessMove>;
            if (threat.Contains(k) && x.Count > 0)
                return true;
            return false;


        }
        public bool IsCheckmate
        {
            get { return checkmate(); }
        }

        public bool IsCheck
        {
            get { return check(); }
        }
        private bool checkmate()
        {
            var x = GetPossibleMoves() as List<ChessMove>;
            BoardPosition k = findKing(CurrentPlayer);
            List<BoardPosition> threat = GetThreatenedPositions(CurrentPlayer == 1 ? 2 : 1) as List<BoardPosition>;
            if (threat.Contains(k) && x.Count == 0)
                return true;
            return false;
        }

        public bool IsStalemate
        {
            get
            {
                var x = GetPossibleMoves() as List<ChessMove>;
                BoardPosition k = findKing(CurrentPlayer);
                List<BoardPosition> threat = GetThreatenedPositions(CurrentPlayer == 1 ? 2 : 1) as List<BoardPosition>;
                if (threat.Contains(k) == false && x.Count == 0)
                    return true;
                return false;
            }
        }

        private bool PassantPossible()
        {
            if (MoveHistory.Count == 0)
                return false;
            ChessMove cm = MoveHistory[MoveHistory.Count - 1] as ChessMove;
            return cm.ForwardTwice;
        }

        private bool hasMoved(BoardPosition bp)
        {
            foreach (ChessMove m in MoveHistory)
            {
                if (m.EndPosition.Equals(bp) && m.pieceval.Equals(mBoard[bp.Row, bp.Col]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool doesContain(List<BoardPosition> l, BoardPosition b)
        {
            foreach (var x in l)
            {
                if (x.Equals(b))
                    return true;
            }
            return false;
        }

        private List<ChessMove> Castling(List<ChessMove> list, BoardPosition kp)
        {
            int side = CurrentPlayer == 1 ? 7 : 0;
            int p = CurrentPlayer == 1 ? 1 : -1;
            List<BoardPosition> threat = GetThreatenedPositions(CurrentPlayer == 1 ? 2 : 1) as List<BoardPosition>;
            BoardPosition kRook = new BoardPosition(side, 7);
            BoardPosition qRook = new BoardPosition(side, 0);
            BoardPosition bp1 = new BoardPosition(side, 1);
            BoardPosition bp2 = new BoardPosition(side, 2);
            BoardPosition bp3 = new BoardPosition(side, 3);
            BoardPosition bp5 = new BoardPosition(side, 5);
            BoardPosition bp6 = new BoardPosition(side, 6);
            //if all spaces empty, if two of the spaces are unthreatened, if king and rook have not moved, and king is unthreatened
            if (PositionIsEmpty(bp1) && PositionIsEmpty(bp2) && PositionIsEmpty(bp3)
                && !doesContain(threat, bp2) && !doesContain(threat, bp3)
                && !hasMoved(kp) && !hasMoved(qRook) && !doesContain(threat, kp) && mBoard[side, 0] == (sbyte)(p * 2))
            {
                //if not under threat
                ChessMove c = new ChessMove(kp, bp2, ChessMoveType.CastleQueenSide);
                c.Piece = GetPieceAtPosition(kp);
                Console.WriteLine(c.Piece.Player.ToString());
                // c.Piece.Player = GetPlayerAtPosition(kp);

                list.Add(c);
            }
            if (PositionIsEmpty(bp5) && PositionIsEmpty(bp6) && !doesContain(threat, bp5) && !doesContain(threat, bp6)
                && !hasMoved(kp) && !hasMoved(kRook) && !doesContain(threat, kp) && mBoard[side, 7] == (sbyte)(p * 3))
            {
                ChessMove c = new ChessMove(kp, bp6, ChessMoveType.CastleKingSide);

                c.Piece = GetPieceAtPosition(kp);
                Console.WriteLine(c.Piece.Player.ToString());
                list.Add(c);
            }
            return list;
            //jsldjfl
        }

        private List<ChessMove> KingPossible(BoardPosition pos)
        {
            //List<BoardPosition> possible = new List<BoardPosition>();
            var possible = KingThreat(pos);
            for (int i = 0; i < possible.Count; i++)
            {
                if (!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition))
                {
                    possible.RemoveAt(i);
                    i--;
                }
            }
            possible = Castling(possible, pos);
            return possible;
        }
        private List<ChessMove> KingThreat(BoardPosition pos)
        {

            //implement king rules on checking and threats
            List<ChessMove> threatened = new List<ChessMove>();
            int player = GetPlayerAtPosition(pos);
            for (int r = pos.Row - 1; r < pos.Row + 2; r++)
            {
                for (int c = pos.Col - 1; c < pos.Col + 2; c++)
                {
                    if (r == 0 && c == 0)
                        continue;
                    else
                    {
                        BoardPosition bp = new BoardPosition(r, c);
                        if (PositionInBounds(bp))
                        {
                            threatened.Add(new ChessMove(pos, bp, mBoard[pos.Row, pos.Col]));
                        }


                    }
                }
            }
            return threatened;
        }

        private List<ChessMove> RookPossible(BoardPosition pos)
        {
            //List<BoardPosition> possible = new List<BoardPosition>();
            var possible = RookThreat(pos);
            for (int i = 0; i < possible.Count; i++)
            {
                if (!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition))
                {
                    possible.RemoveAt(i);
                    i--;
                }
            }

            return possible;

        }


        private List<ChessMove> QueenPossible(BoardPosition pos)
        {
            //List<BoardPosition> possible = new List<BoardPosition>();
            var possible = QueenThreat(pos);
            for (int i = 0; i < possible.Count; i++)
            {
                if (!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition))
                {
                    possible.RemoveAt(i);
                    i--;
                }
            }

            return possible;
        }

        private List<ChessMove> QueenThreat(BoardPosition pos)
        {
            List<ChessMove> threatened = new List<ChessMove>();
            threatened.AddRange(RookThreat(pos));
            threatened.AddRange(BishopThreat(pos));
            return threatened;

        }
        private List<ChessMove> RookThreat(BoardPosition pos)
        {
            List<ChessMove> threatened = new List<ChessMove>();
            //int player = GetPlayerAtPosition(pos);
            for (int r = -1; r < 2; r += 2)
            {
                BoardPosition bp = pos;
                bool bounds = true;
                while (bounds)
                {
                    bp.Row += r;
                    bounds = PositionInBounds(bp);
                    if (!bounds)
                        break;
                    else if (PositionIsEmpty(bp))
                    {
                        threatened.Add(new ChessMove(pos, bp, mBoard[pos.Row, pos.Col]));
                        continue;
                    }
                    else
                    {
                        threatened.Add(new ChessMove(pos, bp, mBoard[pos.Row, pos.Col]));
                        break;
                    }


                }
            }
            for (int c = -1; c < 2; c += 2)
            {
                BoardPosition bp = pos;
                bool bounds = true;
                while (bounds)
                {
                    bp.Col += c;
                    bounds = PositionInBounds(bp);
                    if (!bounds)
                        break;
                    else if (PositionIsEmpty(bp))
                    {
                        threatened.Add(new ChessMove(pos, bp, mBoard[pos.Row, pos.Col]));
                        continue;
                    }
                    else
                    {
                        threatened.Add(new ChessMove(pos, bp, mBoard[pos.Row, pos.Col]));
                        break;
                    }


                }
            }
            return threatened;
        }

        private List<ChessMove> BishopPossible(BoardPosition pos)
        {
            //List<BoardPosition> possible = new List<BoardPosition>();
            var possible = BishopThreat(pos);
            for (int i = 0; i < possible.Count; i++)
            {
                if (!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition))
                {
                    possible.RemoveAt(i);
                    i--;
                }
            }

            return possible;
        }

        private List<ChessMove> BishopThreat(BoardPosition pos)
        {


            List<ChessMove> threatened = new List<ChessMove>();
            //int player = GetPlayerAtPosition(pos);
            for (int r = -1; r < 2; r += 2)
            {
                for (int c = -1; c < 2; c += 2)
                {
                    BoardPosition bp = pos;
                    bool bounds = true;
                    while (bounds)
                    {
                        bp.Row += r;
                        bp.Col += c;
                        bounds = PositionInBounds(bp);
                        if (!bounds) //end if out of bounds
                            break;
                        else if (PositionIsEmpty(bp))
                        { //add and continue if empty
                            threatened.Add(new ChessMove(pos, bp));
                            continue;
                        }
                        else
                        {
                            threatened.Add(new ChessMove(pos, bp)); //add if hit piece and done
                            break;
                        }

                    }
                }
            }
            return threatened;
        }

        private List<ChessMove> KnightPossible(BoardPosition pos)
        {
            //List<BoardPosition> possible = new List<BoardPosition>();
            var possible = KnightThreat(pos);
            for (int i = 0; i < possible.Count; i++)
            {
                if (!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition))
                {

                    possible.RemoveAt(i);
                    i--;
                }
            }

            return possible;
        }
        private List<ChessMove> KnightThreat(BoardPosition pos)
        {
            List<ChessMove> threatened = new List<ChessMove>();
            //int player = GetPlayerAtPosition(pos);
            List<ChessMove> position = new List<ChessMove>(){
                new ChessMove(pos, new BoardPosition(pos.Row+2, pos.Col-1)),
                new ChessMove(pos, new BoardPosition(pos.Row+2, pos.Col+1)),

                new ChessMove(pos, new BoardPosition(pos.Row-2, pos.Col+1)),
                new ChessMove(pos, new BoardPosition(pos.Row-2, pos.Col-1)),

                new ChessMove(pos, new BoardPosition(pos.Row-1, pos.Col+2)),
                new ChessMove(pos, new BoardPosition(pos.Row+1, pos.Col+2)),

                new ChessMove(pos, new BoardPosition(pos.Row-1, pos.Col-2)),
                new ChessMove(pos, new BoardPosition(pos.Row+1, pos.Col-2))};
            foreach (ChessMove bp in position)
            {
                if (PositionInBounds(bp.EndPosition))
                    threatened.Add(bp);
            }
            return threatened;
        }

        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player)
        {
            List<BoardPosition> temp = new List<BoardPosition>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (mBoard[r, c] == GetPieceValue(piece) && GetPlayerAtPosition(new BoardPosition(r, c)) == player)
                    {
                        temp.Add(new BoardPosition(r, c));
                    }
                }
            }
            return temp;
        }
        private List<ChessMove> PawnPossible(BoardPosition position)
        {
            List<ChessMove> possible = PawnThreat(position);
            int player = GetPlayerAtPosition(position), truePlayer = player;
            if (player == 2)
                player = 1;
            else
                player = -1;
            int r = position.Row + player;

            for (int i = 0; i < possible.Count; i++)
            {
                ChessMove m = possible[i];
                if (m.MoveType == ChessMoveType.Normal)
                {
                    if (!PositionIsEnemy(m.EndPosition, CurrentPlayer))
                    {
                        possible.RemoveAt(i);
                        i--;
                    }
                }
                else
                {

                }


            }
            BoardPosition forward = new BoardPosition(r, position.Col);
            BoardPosition forward2 = new BoardPosition(player + r, position.Col);

            if (PositionInBounds(forward) && PositionIsEmpty(forward))
            {
                possible.Add(new ChessMove(position, forward, mBoard[position.Row, position.Col]));
                if (PositionInBounds(forward2) && hasMoved(position) == false && PositionIsEmpty(forward2)
                    && (position.Row == 6 | position.Row == 1))
                {
                    ChessMove cm = new ChessMove(position, forward2, mBoard[position.Row, position.Col]);
                    cm.ForwardTwice = true;
                    possible.Add(cm);
                }
            }

            return possible;
        }
        private List<ChessMove> PawnThreat(BoardPosition position)
        {


            List<ChessMove> threatened = new List<ChessMove>();
            int player = GetPlayerAtPosition(position), truePlayer = player;
            if (player == 2)
                player = 1;
            else
                player = -1;
            int r = position.Row + player;
            BoardPosition forwardLeft = new BoardPosition(r, position.Col - 1);
            BoardPosition forwardRight = new BoardPosition(r, position.Col + 1);


            if (PositionInBounds(forwardLeft))
                threatened.Add(new ChessMove(position, forwardLeft, mBoard[position.Row, position.Col]));
            if (PositionInBounds(forwardRight))
                threatened.Add(new ChessMove(position, forwardRight, mBoard[position.Row, position.Col]));

            //if (PassantPossible())
            //{
            //    ChessMove cm = IsPassant(position);
            //    if (cm.StartPosition.Row != -1)
            //    {
            //        threatened.Add(cm);
            //    }
            //}
            if (MoveHistory.Count > 0)
            {
                ChessMove cm = IsPassant(position);
                if (cm.StartPosition.Row != -1)
                {
                    threatened.Add(cm);
                }
            }
            return threatened;
        }

        private ChessMove IsPassant(BoardPosition position)
        {
            BoardPosition left = new BoardPosition(position.Row, position.Col - 1);
            BoardPosition right = new BoardPosition(position.Row, position.Col + 1);
            int player = GetPlayerAtPosition(position), truePlayer = player;
            if (player == 2)
                player = 1;
            else
                player = -1;
            int r = position.Row + player;
            ChessMove passing = MoveHistory[MoveHistory.Count - 1] as ChessMove;
            if (PositionInBounds(left) && passing.EndPosition.Equals(left))
            {
                ChessMove m3 = new ChessMove(position, new BoardPosition(r, position.Col - 1));
                m3.MoveType = ChessMoveType.EnPassant;
                m3.Captured = GetPieceAtPosition(passing.EndPosition);
                return m3;
            }
            else if (PositionInBounds(right) && passing.EndPosition.Equals(right))
            {
                ChessMove m3 = new ChessMove(position, new BoardPosition(r, position.Col + 1));
                m3.MoveType = ChessMoveType.EnPassant;
                m3.Captured = GetPieceAtPosition(passing.EndPosition);
                return m3;
            }
            return new ChessMove(new BoardPosition(-1, -1), new BoardPosition(-1, -1));
        }

        private List<BoardPosition> GetPlayerPieces(int player)
        {
            List<BoardPosition> list = new List<BoardPosition>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    BoardPosition pos = new BoardPosition(i, j);
                    if (GetPlayerAtPosition(pos) == player)
                    {
                        list.Add(pos);
                    }
                }
            }
            return list;
        }



        /// <summary>
        /// Returns true if the given position on the board is empty.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEmpty(BoardPosition pos)
        {
            if (0 == GetPlayerAtPosition(pos))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the given position contains a piece that is the enemy of the given player.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEnemy(BoardPosition pos, int player)
        {
            if (player != GetPlayerAtPosition(pos) && GetPlayerAtPosition(pos) != 0)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the given position is in the bounds of the board.
        /// </summary>
        public static bool PositionInBounds(BoardPosition pos)
        {
            if (pos.Row >= 0 && pos.Row < 8 && pos.Col >= 0 && pos.Col < 8)
                return true;
            return false;
        }

        /// <summary>
        /// Returns which player has a piece at the given board position, or 0 if it is empty.
        /// </summary>
        public int GetPlayerAtPosition(BoardPosition pos)
        {
            if (mBoard[pos.Row, pos.Col] > 0)
                return 1;
            else if (mBoard[pos.Row, pos.Col] < 0)
                return 2;
            else
                return 0;
        }

        /// <summary>
        /// Gets the value weight for a piece of the given type.
        /// </summary>
        /*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
        public int GetPieceValue(ChessPieceType pieceType)
        {
            if (pieceType == ChessPieceType.Pawn)
                return 1;
            else if (pieceType == ChessPieceType.Knight || pieceType == ChessPieceType.Bishop)
                return 3;
            else if (pieceType == ChessPieceType.RookKing || pieceType == ChessPieceType.RookQueen || pieceType == ChessPieceType.RookPawn)
                return 5;
            else if (pieceType == ChessPieceType.Queen)
                return 9;
            else if (pieceType == ChessPieceType.King)
                return int.MaxValue;
            else
                return 0;
        }


        /// <summary>
        /// Manually places the given piece at the given position.
        /// </summary>
        // This is used in the constructor
        private void SetPosition(BoardPosition position, ChessPiecePosition piece)
        {
            mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 :
                piece.Player));
        }
    }
}