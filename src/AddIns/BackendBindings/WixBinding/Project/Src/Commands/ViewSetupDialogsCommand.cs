// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
