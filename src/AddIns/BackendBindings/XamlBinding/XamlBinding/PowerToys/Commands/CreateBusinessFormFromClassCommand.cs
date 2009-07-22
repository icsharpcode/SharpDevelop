// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.XamlBinding.PowerToys.Dialogs;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	/// <summary>
	/// Description of CreateBusinessFormFromClassCommand
	/// </summary>
	public class CreateBusinessFormFromClassCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			SelectSourceClassDialog selectSourceClass = new SelectSourceClassDialog();
			
			if (selectSourceClass.ShowDialog() ?? false) {
				SourceClassFormEditor editor = new SourceClassFormEditor(selectSourceClass.SelectedClass);
				
				if (editor.ShowDialog() ?? false) {
					return;
				}
			}
		}
	}
}
