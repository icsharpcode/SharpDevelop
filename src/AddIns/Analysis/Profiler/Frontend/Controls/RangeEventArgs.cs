// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
