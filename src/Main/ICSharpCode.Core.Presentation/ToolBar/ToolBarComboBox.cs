// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	sealed class ToolBarComboBox : ComboBox
	{
//		readonly Codon codon;
//		readonly object caller;
		IComboBoxCommand menuCommand;
		
		public ToolBarComboBox(Codon codon, object caller)
		{
			this.IsEditable = false;
			menuCommand = (IComboBoxCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			menuCommand.Run();
		}
	}
}
