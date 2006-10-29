// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class PropertyPadResetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				PropertyPad.Grid.ResetSelectedProperty();
			} catch (Exception e) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Pads.PropertyPadResetCommand}"
				                         + Environment.NewLine + e.Message);
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
