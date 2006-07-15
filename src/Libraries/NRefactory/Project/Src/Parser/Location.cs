/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 15.07.2006
 * Time: 19:49
 */

using System;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Description of Position.
	/// </summary>
	public struct Location
	{
		public static readonly Location Empty = new Location(0, 0);
		
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
	}
}
