// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public delegate void ViewContentEventHandler(object sender, ViewContentEventArgs e);
	
	public class ViewContentEventArgs : EventArgs
	{
		IViewContent content;
		
		public IViewContent Content {
			get {
				return content;
			}
		}
		
		public ViewContentEventArgs(IViewContent content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.content = content;
		}
	}
}
