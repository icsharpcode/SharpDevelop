// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Displays a dialog so the user can browser for PInvoke signatures and
	/// insert one or more of them into the code.
	/// </summary>
	public class InsertPInvokeSignaturesCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command.
		/// </summary>
		public override void Run()
		{
			// Show PInvoke dialog.
			using(InsertPInvokeSignaturesForm form = new InsertPInvokeSignaturesForm()) {
				form.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
	}
}
