// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Dialogs;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of RunExecutable
	/// </summary>
	public class ProfileExecutable : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ProfileExecutableForm form = new ProfileExecutableForm();
			form.Owner = WorkbenchSingleton.MainWindow;
			form.ShowDialog();
		}
	}
}
