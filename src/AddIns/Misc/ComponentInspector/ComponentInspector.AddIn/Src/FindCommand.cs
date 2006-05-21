// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using NoGoop.ObjBrowser.Dialogs;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using System;
using System.Windows.Forms;

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
