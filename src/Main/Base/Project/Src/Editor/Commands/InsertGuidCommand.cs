// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class InsertGuidCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null || !(viewContent is ITextEditorProvider)) {
				return;
			}
			
			ITextEditor textEditor = ((ITextEditorProvider)viewContent).TextEditor;
			if (textEditor == null) {
				return;
			}
			
			string newGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			
			textEditor.SelectedText = newGuid;
			textEditor.Select(textEditor.SelectionStart + textEditor.SelectionLength, 0);
		}
	}
}
