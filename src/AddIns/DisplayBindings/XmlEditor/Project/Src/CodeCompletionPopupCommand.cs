// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using System;
using System.Windows.Forms;

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
