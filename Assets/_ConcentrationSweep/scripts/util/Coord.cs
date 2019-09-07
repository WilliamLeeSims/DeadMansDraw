using System;

public struct Coord : System.IEquatable<Coord>
{
  static public int[] DX = { 1, 1, 0, -1, -1, -1,  0,  1 };
  static public int[] DY = { 0, 1, 1,  1,  0, -1, -1, -1 };

  public int X;
  public int Y;

  public Coord( Coord o ) { X = o.X; Y = o.Y; }
  public Coord( int x, int y ) { X = x; Y = y; }
  public override int GetHashCode() { return X * 1000 + Y; }
  public override bool Equals( object other ) { return other is Coord ? Equals( (Coord)other ) : false; }
  public bool Equals( Coord other ) { return X == other.X && Y == other.Y; }
  public static bool operator ==( Coord a, Coord b )
  {
    if( System.Object.ReferenceEquals( a, b ) )
      return true;
    if( ((object)a == null) || ((object)b == null) )
      return false;
    return a.X == b.X && a.Y == b.Y;
  }
  public static bool operator !=( Coord a, Coord b )
  {
    return !(a == b);
  }
  public void Set( int x, int y )
  {
    X = x;
    Y = y;
  }
  public Coord atAngle( int ang, int dist = 1 )
  {
    return new Coord { X = this.X + dist * DX[ang], Y = this.Y + dist * DY[ang] };
  }
}