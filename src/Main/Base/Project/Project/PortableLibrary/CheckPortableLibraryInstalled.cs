// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	/// <summary>
	/// Shows a message box if the portable library is not installed.
	/// </summary>
	public class CheckPortableLibraryInstalled : AbstractCommand
	{
		public override void Run()
		{
			if (!ProfileList.IsPortableLibraryInstalled()) {
				using (ToolNotFoundDialog dlg = new ToolNotFoundDialog(
					StringParser.Parse(
						"${res:PortableLibrary.CouldNotFindTools}" + Environment.NewLine + Environment.NewLine +
						"${res:PortableLibrary.ToolsInstallationHelp}"),
					"http://go.microsoft.com/fwlink/?LinkId=210823"
				)) {
					// our message is long, so make the window bigger than usual
					dlg.Width += 70;
					dlg.Height += 70;
					dlg.ShowDialog();
				}
			}
		}
	}
}
