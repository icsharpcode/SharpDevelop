// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Actions
{
	public class TemplateCompletion : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			services.AutoClearSelection = false;
			sdtac.ShowCompletionWindow(new TemplateCompletionDataProvider(), '\0');
		}
	}
	
	public class CodeCompletionPopup : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			
			sdtac.ShowCompletionWindow(sdtac.CreateCodeCompletionDataProvider(true), '\0');
		}
	}
	
	public class GoToDefinition : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			TextEditorControl textEditorControl = textArea.MotherTextEditorControl;
			IDocument document = textEditorControl.Document;
			string textContent = document.TextContent;
			
			int caretLineNumber = document.GetLineNumberForOffset(textEditorControl.ActiveTextAreaControl.Caret.Offset) + 1;
			int caretColumn     = textEditorControl.ActiveTextAreaControl.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditorControl.FileName);
			if (expressionFinder == null)
				return;
			string expression = expressionFinder.FindFullExpression(textContent, textEditorControl.ActiveTextAreaControl.Caret.Offset);
			if (expression == null || expression.Length == 0)
				return;
			ResolveResult result = ParserService.Resolve(expression, caretLineNumber, caretColumn, textEditorControl.FileName, textContent);
			if (result != null) {
				FilePosition pos = result.GetDefinitionPosition();
				if (pos != null) {
					try {
						if (pos.Position.IsEmpty)
							FileService.OpenFile(pos.Filename);
						else
							FileService.JumpToFilePosition(pos.Filename, pos.Position.X - 1, pos.Position.Y - 1);
					} catch (Exception ex) {
						Console.WriteLine("Error jumping to '" + pos.Filename + "':\n" + ex.ToString());
						MessageBox.Show("Error jumping to '" + pos.Filename + "':\n" + ex.ToString());
					}
				}
			}
		}
	}
}
