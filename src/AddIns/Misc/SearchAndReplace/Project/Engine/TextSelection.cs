// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace SearchAndReplace
{
	public class TextSelection
	{
		int offset;
		int length;
		
		public TextSelection(int offset, int length)
		{
			this.offset = offset;
			this.length = length;
		}
		
		public int Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		/// <summary>
		/// Checks whether a position is in a specified range.
		/// </summary>
		public static bool IsInsideRange(int position, int offset, int length)
		{
			return position >= offset && position < offset + length;
		}
	}
}
