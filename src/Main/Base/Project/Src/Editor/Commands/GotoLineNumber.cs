// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GotoLineNumber : AbstractMenuCommand
	{
		public override void Run()
		{
			GotoDialog.ShowSingleInstance();
		}
	}
}
