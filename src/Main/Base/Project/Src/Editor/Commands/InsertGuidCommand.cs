// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class InsertGuidCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor == null) {
				return;
			}
			
			string newGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			
			textEditor.SelectedText = newGuid;
			textEditor.Select(textEditor.SelectionStart + textEditor.SelectionLength, 0);
		}
	}
}
