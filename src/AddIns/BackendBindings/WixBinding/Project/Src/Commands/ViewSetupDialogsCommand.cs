// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Opens the setup dialogs pad.
	/// </summary>
	public class ViewSetupDialogsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbench workbench = WorkbenchSingleton.Workbench;
			PadDescriptor setupDialogsPad = workbench.GetPad(typeof(SetupDialogListPad));
			if (setupDialogsPad != null) {
				setupDialogsPad.BringPadToFront();
			} else {
				foreach (PadDescriptor pad in workbench.PadContentCollection) {
					if (pad.PadContent is SetupDialogListPad) {
						workbench.ShowPad(pad);
						break;
					}
				}
			}
		}
	}
}
