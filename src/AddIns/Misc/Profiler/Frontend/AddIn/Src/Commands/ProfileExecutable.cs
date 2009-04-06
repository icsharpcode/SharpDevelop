// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
