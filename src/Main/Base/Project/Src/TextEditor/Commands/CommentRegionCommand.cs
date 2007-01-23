// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class CommentRegion : AbstractMenuCommand
	{ 
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			
			TextEditorControl textarea = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(textarea.ActiveTextAreaControl.TextArea);
		}
	}
}
