namespace FezuChess
{
    public class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Vector2()
        {
            X = 0;
            Y = 0;
        }
        
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public bool IsOutOfBounds()
        {
            return X < 0 || X >= 8 || Y < 0 || Y >= 8;
        }
        
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }
        
        public static Vector2 operator *(Vector2 a, int b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }
        
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
        
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }
        
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Vector2 vector2)
            {
                return X == vector2.X && Y == vector2.Y;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        
        public static Vector2 Up => new(0, 1);
        public static Vector2 Down => new(0, -1);
        public static Vector2 Left => new(-1, 0);
        public static Vector2 Right => new(1, 0);
        public static Vector2 Zero => new(0, 0);
        public static Vector2 UpLeft => Up + Left;
        public static Vector2 UpRight => Up + Right;
        public static Vector2 DownLeft => Down + Left;
        public static Vector2 DownRight => Down + Right;
    }
}