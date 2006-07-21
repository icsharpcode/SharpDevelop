/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 15.07.2006
 * Time: 19:49
 */

using System;

namespace ICSharpCode.NRefactory
{
	/// <summary>
	/// A line/column position.
	/// </summary>
	public struct Location : IComparable<Location>
	{
		public static readonly Location Empty = new Location(-1, -1);
		
		public Location(int column, int line)
		{
			x = column;
			y = line;
		}
		
		int x, y;
		
		public int X {
			get {
				return x;
			}
			set {
				x = value;
			}
		}
		
		public int Y {
			get {
				return y;
			}
			set {
				y = value;
			}
		}
		
		public bool IsEmpty {
			get {
				return x <= 0 && y <= 0;
			}
		}
		
		public override string ToString()
		{
			return string.Format("(Line {1}, Col {0})", this.x, this.y);
		}
		
		public override int GetHashCode()
		{
			return unchecked (87 * x.GetHashCode() ^ y.GetHashCode());
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is Location)) return false;
			return (Location)obj == this;
		}
		
		public static bool operator ==(Location a, Location b)
		{
			return a.x == b.x && a.y == b.y;
		}
		
		public static bool operator !=(Location a, Location b)
		{
			return a.x != b.x || a.y != b.y;
		}
		
		public static bool operator <(Location a, Location b)
		{
			if (a.y < b.y)
				return true;
			else if (a.y == b.y)
				return a.x < b.x;
			else
				return false;
		}
		
		public static bool operator >(Location a, Location b)
		{
			if (a.y > b.y)
				return true;
			else if (a.y == b.y)
				return a.x > b.x;
			else
				return false;
		}
		
		public static bool operator <=(Location a, Location b)
		{
			return !(a > b);
		}
		
		public static bool operator >=(Location a, Location b)
		{
			return !(a < b);
		}
		
		public int CompareTo(Location other)
		{
			if (this == other)
				return 0;
			if (this < other)
				return -1;
			else
				return 1;
		}
	}
}
