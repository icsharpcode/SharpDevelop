// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Actions
{
	public class TemplateCompletion : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)services.MotherTextEditorControl;
			services.AutoClearSelection = false;
			sdtac.ShowCompletionWindow(new TemplateCompletionDataProvider() { AutomaticInsert = true }, '\0');
		}
	}
	
	public class CodeCompletionPopup : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)textArea.MotherTextEditorControl;
			sdtac.StartCtrlSpaceCompletion();
		}
	}
	
	public class ExpandTemplateAction : Tab
	{
		public override void Execute(TextArea textArea)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)textArea.MotherTextEditorControl;
			if (!sdtac.ExpandTemplateOnTab())
				base.Execute(textArea);
		}
	}
	
	#if DEBUG
	internal class DebugCtrlSpaceCodeCompletionAction : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)textArea.MotherTextEditorControl;
			CtrlSpaceCompletionDataProvider provider = new CtrlSpaceCompletionDataProvider();
			provider.AllowCompleteExistingExpression = true;
			provider.DebugMode = true;
			sdtac.ShowCompletionWindow(provider, '\0');
		}
	}
	
	internal class DebugDotCompletionAction : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			SharpDevelopTextAreaControl sdtac = (SharpDevelopTextAreaControl)textArea.MotherTextEditorControl;
			CodeCompletionDataProvider ccdp = new CodeCompletionDataProvider();
			ccdp.DebugMode = true;
			sdtac.ShowCompletionWindow(ccdp, '.');
		}
	}
	#endif
	
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
			ExpressionResult expression = expressionFinder.FindFullExpression(textContent, textEditorControl.ActiveTextAreaControl.Caret.Offset);
			if (expression.Expression == null || expression.Expression.Length == 0)
				return;
			ResolveResult result = ParserService.Resolve(expression, caretLineNumber, caretColumn, textEditorControl.FileName, textContent);
			if (result != null) {
				FilePosition pos = result.GetDefinitionPosition();
				if (pos.IsEmpty == false) {
					try {
						if (pos.Position.IsEmpty)
							FileService.OpenFile(pos.FileName);
						else
							FileService.JumpToFilePosition(pos.FileName, pos.Line - 1, pos.Column - 1);
					} catch (Exception ex) {
						MessageService.ShowError(ex, "Error jumping to '" + pos.FileName + "'.");
					}
				}
			}
		}
	}
}
