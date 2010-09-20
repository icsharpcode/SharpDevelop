// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.Svn.Commands
{
	public class ExportCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowExportDialog(null);
		}
	}
	
	public class CheckoutCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowCheckoutDialog(null);
		}
	}
}
