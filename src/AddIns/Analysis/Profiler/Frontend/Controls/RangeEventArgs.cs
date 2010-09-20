// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.Profiler.Controls
{
	public class RangeEventArgs : EventArgs
	{
		int startIndex, endIndex;
	
		public int EndIndex
		{
			get { return endIndex; }
		}
	
		public int StartIndex
		{
			get { return startIndex; }
		}
	
		public RangeEventArgs(int startIndex, int endIndex)
		{
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}
	
		public int RangeLength
		{
			get { return Math.Abs(this.endIndex - this.startIndex); }
		}
	}
}
