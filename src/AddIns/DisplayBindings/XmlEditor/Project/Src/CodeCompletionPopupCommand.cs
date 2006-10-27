// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Command executed when the user hits Ctrl+Space
	/// </summary>
	public class  CodeCompletionPopupCommand : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			XmlEditorControl editor = services.MotherTextEditorControl as XmlEditorControl;
			if (editor != null) {
				editor.ShowCompletionWindow();			
			}
		}
	}
}
