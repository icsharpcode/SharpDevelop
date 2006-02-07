// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
