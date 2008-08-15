// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor.Document;
using SharpRefactoring.Forms;
using SharpRefactoring.Transformers;
using SharpRefactoring.Visitors;

namespace SharpRefactoring
{
	public class ExtractMethodCommand : AbstractRefactoringCommand
	{
		protected override void Run(ICSharpCode.TextEditor.TextEditorControl textEditor, ICSharpCode.SharpDevelop.Dom.Refactoring.RefactoringProvider provider)
		{
			if (textEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
			{
				MethodExtractorBase extractor = GetCurrentExtractor(textEditor);
				if (extractor != null) {
					if (extractor.Extract()) {
						ExtractMethodForm form = new ExtractMethodForm("NewMethod", extractor.CreatePreview());
						
						if (form.ShowDialog() == DialogResult.OK) {
							extractor.ExtractedMethod.Name = form.Text;
							textEditor.Document.UndoStack.StartUndoGroup();
							extractor.InsertAfterCurrentMethod();
							extractor.InsertCall();
							textEditor.Document.FormattingStrategy.IndentLines(textEditor.ActiveTextAreaControl.TextArea, 0, textEditor.Document.TotalNumberOfLines - 1);
							textEditor.Document.UndoStack.EndUndoGroup();
							textEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
						}
					}
				}
			}
		}
		
		MethodExtractorBase GetCurrentExtractor(TextEditorControl editor)
		{
			switch (ProjectService.CurrentProject.Language) {
				case "C#":
					return new CSharpMethodExtractor(editor, editor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0]);
				default:
					MessageService.ShowError(string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.ExtractMethodNotSupported}"), ProjectService.CurrentProject.Language));
					return null;
			}
		}
	}
}
