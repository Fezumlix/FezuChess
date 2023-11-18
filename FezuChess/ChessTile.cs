using System.Drawing;

namespace FezuChess
{
    public class ChessTile : System.Windows.Forms.Panel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2 Position => new(X, Y);
        private bool IsWhite => (X + Y) % 2 == 1;
        private TileStatus _status;

        public TileStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                BackColor = _status switch
                {
                    TileStatus.Normal => IsWhite ? Color.DarkGray : Color.DimGray,
                    TileStatus.Selected => Color.LightBlue,
                    TileStatus.PossibleMove => Color.LightGreen,
                    TileStatus.PossibleCapture => Color.LightCoral,
                    _ => BackColor
                };
            }
        }
        
        public ChessTile(int x, int y)
        {
            X = x;
            Y = y;
            Size = new Size(50, 50);
            Location = new Point(x * 50, 350 - y * 50);
            Status = TileStatus.Normal;
        }
    }
    
    public enum TileStatus
    {
        Normal,
        Selected,
        PossibleMove,
        PossibleCapture
    }
}