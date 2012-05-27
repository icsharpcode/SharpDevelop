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
					"Could not find Portable Class Library Tools." + Environment.NewLine + Environment.NewLine +
					"To install the Portable Class Library Tools without installing Visual Studio, save the download file (PortableLibraryTools.exe) on your computer, and run the installation program from a Command Prompt window. Include the /buildmachine switch on the command line.",
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
