// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToMatchingBrace : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider editorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (editorProvider != null) {
				Run(editorProvider.TextEditor, editorProvider.TextEditor.Caret.Offset);
			}
		}
		
		public static void Run(ITextEditor editor, int offset)
		{
			BracketSearchResult result = editor.Language.BracketSearcher.SearchBracket(editor.Document, offset);
			if (result != null) {
				// place caret after the other bracket
				if (result.OpeningBracketOffset <= offset && offset <= result.OpeningBracketOffset + result.OpeningBracketLength) {
					editor.Select(result.ClosingBracketOffset + result.ClosingBracketLength, 0);
				} else {
					editor.Select(result.OpeningBracketOffset + result.OpeningBracketLength, 0);
				}
			}
		}
	}
}
