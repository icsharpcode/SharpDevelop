// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Util
{
	public delegate void LineReceivedEventHandler(object sender, LineReceivedEventArgs e);
	
	/// <summary>
	/// The arguments for the <see cref="LineReceivedEventHandler"/> event.
	/// </summary>
	public class LineReceivedEventArgs : EventArgs
	{
		string line = String.Empty;
		
		public LineReceivedEventArgs(string line)
		{
			this.line = line;
		}
		
		public string Line {
			get {
				return line;
			}
		}
	}
}
