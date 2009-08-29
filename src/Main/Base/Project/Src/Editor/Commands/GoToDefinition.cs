// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : AbstractMenuCommand
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
			IDocument document = editor.Document;
			string textContent = document.Text;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null)
				return;
			ExpressionResult expression = expressionFinder.FindFullExpression(textContent, offset);
			if (expression.Expression == null || expression.Expression.Length == 0)
				return;
			var caretPos = editor.Document.OffsetToPosition(offset);
			ResolveResult result = ParserService.Resolve(expression, caretPos.Line, caretPos.Column, editor.FileName, textContent);
			if (result != null) {
				FilePosition pos = result.GetDefinitionPosition();
				if (pos.IsEmpty == false) {
					try {
						if (pos.Position.IsEmpty)
							FileService.OpenFile(pos.FileName);
						else
							FileService.JumpToFilePosition(pos.FileName, pos.Line, pos.Column);
					} catch (Exception ex) {
						MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
					}
				}
			}
		}
	}
}
