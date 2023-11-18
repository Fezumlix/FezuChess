using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FezuChess
{
    public partial class Form1 : Form
    {
        ChessTile[,] grid = new ChessTile[8, 8];
        ChessState state = new();
        
        List<(Vector2, PieceType, bool)> startingPieces = new()
        {
            // White
            (new Vector2(0, 0), PieceType.Rook, true),
            (new Vector2(1, 0), PieceType.Knight, true),
            (new Vector2(2, 0), PieceType.Bishop, true),
            (new Vector2(3, 0), PieceType.Queen, true),
            (new Vector2(4, 0), PieceType.King, true),
            (new Vector2(5, 0), PieceType.Bishop, true),
            (new Vector2(6, 0), PieceType.Knight, true),
            (new Vector2(7, 0), PieceType.Rook, true),
            (new Vector2(0, 1), PieceType.Pawn, true),
            (new Vector2(1, 1), PieceType.Pawn, true),
            (new Vector2(2, 1), PieceType.Pawn, true), 
            (new Vector2(3, 1), PieceType.Pawn, true),
            (new Vector2(4, 1), PieceType.Pawn, true),
            (new Vector2(5, 1), PieceType.Pawn, true),
            (new Vector2(6, 1), PieceType.Pawn, true),
            (new Vector2(7, 1), PieceType.Pawn, true),
            // Black
            (new Vector2(0, 7), PieceType.Rook, false),
            (new Vector2(1, 7), PieceType.Knight, false),
            (new Vector2(2, 7), PieceType.Bishop, false),
            (new Vector2(3, 7), PieceType.Queen, false),
            (new Vector2(4, 7), PieceType.King, false),
            (new Vector2(5, 7), PieceType.Bishop, false),
            (new Vector2(6, 7), PieceType.Knight, false),
            (new Vector2(7, 7), PieceType.Rook, false),
            (new Vector2(0, 6), PieceType.Pawn, false),
            (new Vector2(1, 6), PieceType.Pawn, false),
            (new Vector2(2, 6), PieceType.Pawn, false),
            (new Vector2(3, 6), PieceType.Pawn, false),
            (new Vector2(4, 6), PieceType.Pawn, false),
            (new Vector2(5, 6), PieceType.Pawn, false), 
            (new Vector2(6, 6), PieceType.Pawn, false),
            (new Vector2(7, 6), PieceType.Pawn, false),
        };

        private Dictionary<string, Image> Pieces = new();

        public Form1()
        {
            InitializeComponent();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    grid[x, y] = new ChessTile(x, y);
                    panelChess.Controls.Add(grid[x, y]);
                    grid[x, y].Click += Tile_Click;
                }
            }
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assemblyPath.Substring(0, assemblyPath.Length - 9), "Pieces");
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var image = Image.FromFile(file);
                image = new Bitmap(image, new Size(50, 50));
                Pieces.Add(name, image);
            }
            
            PopulateGrid();
            UpdateGrid();
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                ResetGame();
            }
        }

        private void Tile_Click(object sender, EventArgs e)
        {
            var tile = sender as ChessTile;
            var piece = state.Pieces[tile.X, tile.Y];
            
            // possiblities here are:
            // 1. no piece selected, click on a piece of your color
            // 2. no piece selected, click on a piece of the other color (do nothing)
            // 3. no piece selected, click on an empty tile (do nothing)
            // 4. piece selected, click on a piece of your color (select the new piece)
            // 5. piece selected, click on a piece of the other color if the piece can move there (move to the tile and remove the piece)
            // 6. piece selected, click on a piece of the other color if the piece cannot move there (do nothing)
            // 7. piece selected, click on an empty tile if the piece can move there (move to the tile)
            // 8. piece selected, click on an empty tile if the piece cannot move there (do nothing)
            
            if (state.SelectedPiece == null)
            {
                if (piece is not null && piece.IsWhite == state.IsWhiteTurn)
                {
                    SelectPiece(piece);
                }
            }
            else
            {
                if (piece is not null)
                {
                    if (piece == state.SelectedPiece)
                    {
                        DeselectPiece();
                    }
                    else if (piece.IsWhite == state.SelectedPiece.IsWhite)
                    {
                        SelectPiece(piece);
                    }
                    else if (state.PossibleMoves.Contains(tile.Position))
                    {
                        MovePiece(state.SelectedPiece.Position, tile.Position);
                    }
                }
                else if (state.PossibleMoves.Contains(tile.Position))
                {
                    MovePiece(state.SelectedPiece.Position, tile.Position);
                }
            }
        }

        private void MovePiece(Vector2 origin, Vector2 target)
        {
            grid[state.SelectedPiece.Position.X, state.SelectedPiece.Position.Y].Status = TileStatus.Normal;
            
            state.MovePiece(origin, target, true);
            
            DeselectPiece();
            UpdateGrid();
            // change background color
            BackColor = state.IsWhiteTurn ? Color.White : Color.Black;
            
            // check for checkmate
            if (state.IsInCheckmate(state.IsWhiteTurn))
            {
                MessageBox.Show($"Checkmate! {(!state.IsWhiteTurn ? "White" : "Black")} wins!\nIf you close this window, the game will reset.");
                ResetGame();
            }
        }

        private void SelectPiece(ChessPiece piece)
        {
            if (state.SelectedPiece != null) DeselectPiece();
            
            state.SelectedPiece = piece;
            grid[piece.Position.X, piece.Position.Y].Status = TileStatus.Selected;
            var possibleMoves = piece.GetPossibleMoves(state);
            foreach (var move in possibleMoves)
            {
                grid[move.X, move.Y].Status = state.Pieces[move.X, move.Y] is null ? TileStatus.PossibleMove : TileStatus.PossibleCapture;
            }
            state.PossibleMoves = possibleMoves;
        }
        
        private void DeselectPiece()
        {
            grid[state.SelectedPiece.Position.X, state.SelectedPiece.Position.Y].Status = TileStatus.Normal;
            state.SelectedPiece = null;
            foreach (var move in state.PossibleMoves)
            {
                grid[move.X, move.Y].Status = TileStatus.Normal;
            }
            state.PossibleMoves = new List<Vector2>();
        }

        void PopulateGrid()
        {
            foreach (var piece in startingPieces)
            {
                state.Pieces[piece.Item1.X, piece.Item1.Y] = new ChessPiece(piece.Item1, piece.Item2, piece.Item3);
            }
        }

        void UpdateGrid()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var tile = grid[x, y];
                    var piece = state.Pieces[x, y];
                    tile.BackgroundImage = piece is null ? null : GetImage(piece.Type, piece.IsWhite);
                }
            }
            
            Image GetImage(PieceType _type, bool isWhite)
            {
                string color = isWhite ? "w" : "b";
                string type = _type switch
                {
                    PieceType.Pawn => "p",
                    PieceType.Rook => "r",
                    PieceType.Knight => "n",
                    PieceType.Bishop => "b",
                    PieceType.Queen => "q",
                    PieceType.King => "k",
                    _ => throw new IndexOutOfRangeException()
                };
                return Pieces[$"{type}_{color}"];
            }
        }

        void ResetGame()
        {
            foreach (var tile in grid)
            {
                tile.Status = TileStatus.Normal;
            }
            state = new ChessState();
            PopulateGrid();
            UpdateGrid();
            BackColor = Color.White;
        }
    }

    internal class ChessState : ICloneable
    {
        public ChessPiece[,] Pieces { get; set; } = new ChessPiece[8, 8];
        public bool IsWhiteTurn { get; set; } = true;
        public ChessPiece SelectedPiece { get; set; }
        public List<Vector2> PossibleMoves { get; set; } = new();
        public (Vector2, Vector2, ChessPiece) LastMove { get; set; } = (new(), new(), null);
        
        public ChessPiece GetPiece(Vector2 position)
        {
            if (position.IsOutOfBounds())
            {
                return null;
            }
            return Pieces[position.X, position.Y];
        }

        public void MovePiece(Vector2 origin, Vector2 target, bool withPromotion = false)
        {
            // check for castling
            if (Pieces[origin.X, origin.Y].Type == PieceType.King && Math.Abs(origin.X - target.X) == 2)
            {
                if (target.X == 6)
                {
                    MovePiece(new Vector2(7, origin.Y), new Vector2(5, origin.Y));
                }
                else if (target.X == 2)
                {
                    MovePiece(new Vector2(0, origin.Y), new Vector2(3, origin.Y));
                }
            }
            
            // check for en passant
            if (Pieces[origin.X, origin.Y].Type == PieceType.Pawn && Pieces[target.X, target.Y] is null && target.X != origin.X)
            {
                Pieces[target.X, origin.Y] = null;
            }
            
            Pieces[target.X, target.Y] = Pieces[origin.X, origin.Y];
            Pieces[origin.X, origin.Y] = null;
            Pieces[target.X, target.Y].Position = target;
            Pieces[target.X, target.Y].Moved = true;
            LastMove = (origin, target, Pieces[target.X, target.Y]);
            
            // promote pawn if necessary
            if (withPromotion && Pieces[target.X, target.Y].Type == PieceType.Pawn && target.Y is 0 or 7)
            {
                // create a "PromotionForm"
                var form = new PromotionForm();
                form.ShowDialog();
                Pieces[target.X, target.Y].Type = form.PromotionType;
            }

            IsWhiteTurn = !IsWhiteTurn;
        }
        
        public bool IsInCheck(bool white)
        {
            Console.WriteLine(Pieces.Cast<ChessPiece>());
            Vector2 kingPos = Pieces.Cast<ChessPiece>().First(p => p is not null && p.Type == PieceType.King && p.IsWhite == white).Position;

            return Pieces.Cast<ChessPiece>().Where(p => p is not null && p.IsWhite != white).Any(p => p.GetPossibleMoves(this, false).Contains(kingPos));
        }
        
        public bool IsInCheckmate(bool white)
        {
            if (!IsInCheck(white)) return false;
            return !Pieces.Cast<ChessPiece>().Any(p => p is not null && p.IsWhite == white && p.GetPossibleMoves(this).Any());
        }

        public object Clone()
        {
            var clone = new ChessState();
            var newPieces = new ChessPiece[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    newPieces[x, y] = Pieces[x, y] is null ? null : new ChessPiece(Pieces[x, y].Position, Pieces[x, y].Type, Pieces[x, y].IsWhite);
                }
            }
            clone.Pieces = newPieces;
            clone.IsWhiteTurn = IsWhiteTurn;
            clone.SelectedPiece = SelectedPiece;
            clone.PossibleMoves = PossibleMoves;
            return clone;
        }
    }

    internal class ChessPiece
    {
        public PieceType Type { get; set; }
        public bool IsWhite { get; set; }
        public bool Moved { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Forward => IsWhite ? Vector2.Up : Vector2.Down;
        
        public ChessPiece(Vector2 position, PieceType type, bool isWhite)
        {
            Type = type;
            IsWhite = isWhite;
            Position = position;
        }

        public List<Vector2> GetPossibleMoves(ChessState state, bool checkForCheck = true)
        {
            List<Vector2> moves = new List<Vector2>();
            switch (Type)
            {
                case PieceType.Pawn:
                    if (state.GetPiece(Position + Forward) == null)
                    {
                        moves.Add(Position + Forward);
                        if (!Moved && state.GetPiece(Position + Forward * 2) == null)
                        {
                            moves.Add(Position + Forward * 2);
                        }
                    }
                    if (state.GetPiece(Position + Forward + Vector2.Left) is not null && state.GetPiece(Position + Forward + Vector2.Left).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Forward + Vector2.Left);
                    }
                    if (state.GetPiece(Position + Forward + Vector2.Right) is not null && state.GetPiece(Position + Forward + Vector2.Right).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Forward + Vector2.Right);
                    }
                    
                    // en passant
                    if (state.LastMove.Item3 is not null && state.LastMove.Item3.Type == PieceType.Pawn &&
                        state.LastMove.Item3.IsWhite != IsWhite &&
                        Math.Abs(state.LastMove.Item1.Y - state.LastMove.Item2.Y) == 2)
                    {
                        if (Position + Vector2.Left == state.LastMove.Item2)
                        {
                            moves.Add(Position + Vector2.Left + Forward);
                        }
                        else if (Position + Vector2.Right == state.LastMove.Item2)
                        {
                            moves.Add(Position + Vector2.Right + Forward);
                        }
                    }
                    break;
                case PieceType.Rook:
                    for (int m = 0; m < 4; m++)
                    {
                        Vector2 move = m switch
                        {
                            0 => Vector2.Up,
                            1 => Vector2.Down,
                            2 => Vector2.Left,
                            3 => Vector2.Right,
                            _ => throw new IndexOutOfRangeException()
                        };
                        for (int i = 1; i < 8; i++)
                        {
                            if (state.GetPiece(Position + move * i) is null)
                            {
                                moves.Add(Position + move * i);
                            }
                            else
                            {
                                if (state.GetPiece(Position + move * i).IsWhite != IsWhite)
                                {
                                    moves.Add(Position + move * i);
                                }
                                break;
                            }
                        }
                    }
                    break;
                case PieceType.Bishop:
                    for (int m = 0; m < 4; m++)
                    {
                        Vector2 move = m switch
                        {
                            0 => Vector2.Up + Vector2.Left,
                            1 => Vector2.Up + Vector2.Right,
                            2 => Vector2.Down + Vector2.Left,
                            3 => Vector2.Down + Vector2.Right,
                            _ => throw new IndexOutOfRangeException()
                        };
                        for (int i = 1; i < 8; i++)
                        {
                            if (state.GetPiece(Position + move * i) is null)
                            {
                                moves.Add(Position + move * i);
                            }
                            else
                            {
                                if (state.GetPiece(Position + move * i).IsWhite != IsWhite)
                                {
                                    moves.Add(Position + move * i);
                                }
                                break;
                            }
                        }
                    }
                    break;
                case PieceType.Knight:
                    if (state.GetPiece(Position + Vector2.Up * 2 + Vector2.Left) is null || state.GetPiece(Position + Vector2.Up * 2 + Vector2.Left).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Up * 2 + Vector2.Left);
                    }
                    if (state.GetPiece(Position + Vector2.Up * 2 + Vector2.Right) is null || state.GetPiece(Position + Vector2.Up * 2 + Vector2.Right).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Up * 2 + Vector2.Right);
                    }
                    if (state.GetPiece(Position + Vector2.Down * 2 + Vector2.Left) is null || state.GetPiece(Position + Vector2.Down * 2 + Vector2.Left).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Down * 2 + Vector2.Left);
                    }
                    if (state.GetPiece(Position + Vector2.Down * 2 + Vector2.Right) is null || state.GetPiece(Position + Vector2.Down * 2 + Vector2.Right).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Down * 2 + Vector2.Right);
                    }
                    if (state.GetPiece(Position + Vector2.Left * 2 + Vector2.Up) is null || state.GetPiece(Position + Vector2.Left * 2 + Vector2.Up).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Left * 2 + Vector2.Up);
                    }
                    if (state.GetPiece(Position + Vector2.Left * 2 + Vector2.Down) is null || state.GetPiece(Position + Vector2.Left * 2 + Vector2.Down).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Left * 2 + Vector2.Down);
                    }
                    if (state.GetPiece(Position + Vector2.Right * 2 + Vector2.Up) is null || state.GetPiece(Position + Vector2.Right * 2 + Vector2.Up).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Right * 2 + Vector2.Up);
                    }
                    if (state.GetPiece(Position + Vector2.Right * 2 + Vector2.Down) is null || state.GetPiece(Position + Vector2.Right * 2 + Vector2.Down).IsWhite != IsWhite)
                    {
                        moves.Add(Position + Vector2.Right * 2 + Vector2.Down);
                    }
                    break;
                case PieceType.Queen:
                    for (int m = 0; m < 8; m++)
                    {
                        Vector2 move = m switch
                        {
                            0 => Vector2.Up,
                            1 => Vector2.Down,
                            2 => Vector2.Left,
                            3 => Vector2.Right,
                            4 => Vector2.UpLeft,
                            5 => Vector2.UpRight,
                            6 => Vector2.DownLeft,
                            7 => Vector2.DownRight,
                            _ => throw new IndexOutOfRangeException()
                        };
                        for (int i = 1; i < 8; i++)
                        {
                            if (state.GetPiece(Position + move * i) is null)
                            {
                                moves.Add(Position + move * i);
                            }
                            else
                            {
                                if (state.GetPiece(Position + move * i).IsWhite != IsWhite)
                                {
                                    moves.Add(Position + move * i);
                                }
                                break;
                            }
                        }
                    }
                    break;
                case PieceType.King:
                    for (int m = 0; m < 8; m++)
                    {
                        Vector2 move = m switch
                        {
                            0 => Vector2.Up,
                            1 => Vector2.Down,
                            2 => Vector2.Left,
                            3 => Vector2.Right,
                            4 => Vector2.UpLeft,
                            5 => Vector2.UpRight,
                            6 => Vector2.DownLeft,
                            7 => Vector2.DownRight,
                            _ => throw new IndexOutOfRangeException()
                        };
                        if (state.GetPiece(Position + move) is null || state.GetPiece(Position + move).IsWhite != IsWhite)
                        {
                            moves.Add(Position + move);
                        }
                    }
                    // Castling
                    ChessPiece rook = state.GetPiece(Position + Vector2.Left * 4);
                    if (rook is not null && rook.Type == PieceType.Rook && !rook.Moved && state.GetPiece(Position + Vector2.Left * 3) is null && state.GetPiece(Position + Vector2.Left * 2) is null && state.GetPiece(Position + Vector2.Left) is null)
                    {
                        moves.Add(Position + Vector2.Left * 2);
                    }
                    rook = state.GetPiece(Position + Vector2.Right * 3);
                    if (rook is not null && rook.Type == PieceType.Rook && !rook.Moved && state.GetPiece(Position + Vector2.Right * 2) is null && state.GetPiece(Position + Vector2.Right) is null)
                    {
                        moves.Add(Position + Vector2.Right * 2);
                    }
                    break;
            }
            
            moves = moves.Where(m => !m.IsOutOfBounds()).ToList();

            // Check for check
            if (checkForCheck)
            {
                for (var i = 0; i < moves.Count; i++)
                {
                    var move = moves[i];
                    // create a copy of the state and make the move. Then check if the king is in check
                    ChessState copy = state.Clone() as ChessState;
                    copy.MovePiece(Position, move);
                    if (copy.IsInCheck(IsWhite))
                    {
                        moves.RemoveAt(i);
                        i--;
                    }
                }
            }
            
            return moves;
        }
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
}