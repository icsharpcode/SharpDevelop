// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
