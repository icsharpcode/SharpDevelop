// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace AddInScout
{
	public class AddInScoutCommand : AbstractMenuCommand
	{
		public override void Run() 
		{
			AddInScoutViewContent vw = new AddInScoutViewContent();
			WorkbenchSingleton.Workbench.ShowView(vw);
		}
	}
}
