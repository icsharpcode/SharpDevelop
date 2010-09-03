// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
