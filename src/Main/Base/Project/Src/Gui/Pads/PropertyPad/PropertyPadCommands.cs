// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	}
}
