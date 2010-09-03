// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace Plugins.RegExpTk {
	
	public class RegExpTkCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			RegExpTkDialog dialog = new RegExpTkDialog();
			dialog.Show(WorkbenchSingleton.MainWin32Window);
		}
	}
}
