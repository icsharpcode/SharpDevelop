// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class PropertyPadResetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				PropertyPad.Grid.ResetSelectedProperty();
			} catch (Exception e) {
				MessageService.ShowError(e, "${res:ICSharpCode.SharpDevelop.Gui.Pads.PropertyPadResetCommand}");
			}
		}
	}
	
	public class PropertyPadShowDescriptionCommand : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return PropertyPad.Grid.HelpVisible;
			}
			set {
				PropertyPad.Grid.HelpVisible = value;
			}
		}
		
		public override void Run()
		{
			PropertyPad.Grid.HelpVisible = !PropertyPad.Grid.HelpVisible;
		}
	}
}
