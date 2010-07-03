// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger;
using Debugger.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class SelectLanguageCommand : AbstractComboBoxCommand
	{
		ConsolePad pad;
		ComboBox box;
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			this.pad = this.Owner as ConsolePad;
			if (this.pad == null)
				return;
			
			box = this.ComboBox as ComboBox;
			
			if (this.box == null)
				return;
			
			foreach (var name in Enum.GetNames(typeof(SupportedLanguage)))
				box.Items.Add(name);
			
			box.SelectedIndex = 0;
			
			base.OnOwnerChanged(e);
		}
		
		public override void Run()
		{
			if (this.pad != null && this.box != null) {
				pad.SelectedLanguage = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), box.SelectedValue.ToString());
			}
			base.Run();
		}
	}
}
