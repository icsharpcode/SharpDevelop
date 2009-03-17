// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.AstBuilder;
using System;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class PasteAsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			string clipboardText = ClipboardWrapper.GetText();
			if (string.IsNullOrEmpty(clipboardText))
				return;
			
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			
			TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			if (textEditor == null) {
				return;
			}
			
			textEditor.BeginUpdate();
			textEditor.Document.UndoStack.StartUndoGroup();
			try {
				Run(textEditor, clipboardText);
			} finally {
				textEditor.Document.UndoStack.EndUndoGroup();
				textEditor.EndUpdate();
			}
			textEditor.Refresh();
		}
		
		protected abstract void Run(TextEditorControl editor, string clipboardText);
		
		protected string GetIndentation(ICSharpCode.TextEditor.Document.IDocument document, int line)
		{
			string lineText = document.GetText(document.GetLineSegment(line));
			return lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);
		}
	}
	
	/// <summary>
	/// Pastes the clipboard text as a comment.
	/// 
	/// Does the following:
	/// - Take clipboard text
	/// - Get current indentation
	/// - Wrap first line using 'IAmbience.WrapComment'
	/// - If it's too long (according to the column ruler position), word-break
	/// - Insert it
	/// </summary>
	public class PasteAsCommentCommand : PasteAsCommand
	{
		protected override void Run(TextEditorControl editor, string clipboardText)
		{
			TextArea textArea = editor.ActiveTextAreaControl.TextArea;
			string indentation = GetIndentation(editor.Document, textArea.Caret.Line);
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			int maxLineLength = textArea.TextEditorProperties.VerticalRulerRow - VisualIndentationLength(textArea, indentation);
			StringBuilder insertedText = new StringBuilder();
			using (StringReader reader = new StringReader(clipboardText)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					AppendTextLine(indentation, ambience, maxLineLength, insertedText, line);
				}
			}
			var document = textArea.Document;
			int insertionPos = document.GetLineSegment(textArea.Caret.Line).Offset + indentation.Length;
			document.Insert(insertionPos, insertedText.ToString());
		}
		
		void AppendTextLine(string indentation, IAmbience ambience, int maxLineLength, StringBuilder insertedText, string line)
		{
			const int minimumLineLength = 10;
			string commentedLine;
			while (true) {
				commentedLine = ambience.WrapComment(line);
				int commentingOverhead = commentedLine.Length - line.Length;
				if (commentingOverhead < 0 || (maxLineLength - commentingOverhead) < minimumLineLength)
					break;
				if (commentedLine.Length > maxLineLength) {
					int pos = FindWrapPositionBefore(line, maxLineLength - commentingOverhead);
					if (pos < minimumLineLength)
						break;
					insertedText.AppendLine(ambience.WrapComment(line.Substring(0, pos)));
					insertedText.Append(indentation);
					line = line.Substring(pos + 1);
				} else {
					break;
				}
			}
			insertedText.AppendLine(commentedLine);
			insertedText.Append(indentation); // indentation for next line
		}
		
		int FindWrapPositionBefore(string line, int pos)
		{
			return line.LastIndexOf(' ', pos);
		}
		
		int VisualIndentationLength(TextArea textArea, string indentation)
		{
			int length = 0;
			foreach (char c in indentation) {
				if (c == '\t')
					length += textArea.TextEditorProperties.TabIndent;
				else
					length += 1;
			}
			return length;
		}
	}
	
	/// <summary>
	/// Pastes the clipboard text as a string.
	/// 
	/// Does the following:
	/// - Take clipboard text
	/// - Convert to string literal using CodeGenerator
	/// - Insert it
	/// </summary>
	public class PasteAsStringCommand : PasteAsCommand
	{
		protected override void Run(TextEditorControl editor, string clipboardText)
		{
			CodeGenerator codeGenerator = ParserService.CurrentProjectContent.Language.CodeGenerator;
			if (codeGenerator == null)
				codeGenerator = LanguageProperties.CSharp.CodeGenerator;
			Expression expression = null;
			using (StringReader reader = new StringReader(clipboardText)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					Expression newExpr = new PrimitiveExpression(line);
					if (expression == null) {
						expression = newExpr;
					} else {
						expression = expression
							.Operator(BinaryOperatorType.Concat,
							          ExpressionBuilder.Identifier("Environment").Member("NewLine"))
							.Operator(BinaryOperatorType.Concat,
							          newExpr);
					}
				}
			}
			if (expression == null)
				return;
			TextArea textArea = editor.ActiveTextAreaControl.TextArea;
			string indentation = GetIndentation(editor.Document, textArea.Caret.Line);
			textArea.InsertString(codeGenerator.GenerateCode(expression, indentation).Trim());
		}
	}
}
