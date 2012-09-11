// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
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
