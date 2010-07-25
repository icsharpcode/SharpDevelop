// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using NoGoop.ObjBrowser.Dialogs;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class FindCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindDialog.DoShowDialog();
		}
	}
}
