// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Forms;

namespace SharpRefactoring
{
	public class ExtractMethodCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			if (textEditor.SelectionLength > 0) {
				
				MethodExtractorBase extractor = GetCurrentExtractor(textEditor);
				if (extractor != null) {
					if (extractor.Extract()) {
						ExtractMethodForm form = new ExtractMethodForm(extractor.ExtractedMethod, new Func<IOutputAstVisitor>(extractor.GetOutputVisitor));
						
						if (form.ShowDialog() == DialogResult.OK) {
							extractor.ExtractedMethod.Name = form.Text;
							using (textEditor.Document.OpenUndoGroup()) {
								extractor.InsertAfterCurrentMethod();
								extractor.InsertCall();
								textEditor.Language.FormattingStrategy.IndentLines(textEditor, 0, textEditor.Document.TotalNumberOfLines - 1);
							}
							textEditor.Select(textEditor.SelectionStart, 0);
						}
					}
				}
			}
		}
		
		MethodExtractorBase GetCurrentExtractor(ITextEditor editor)
		{
			switch (ProjectBindingService.GetCodonPerCodeFileName(editor.FileName).Language) {
				case "C#":
					return new CSharpMethodExtractor(editor);
				default:
					MessageService.ShowError(string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.ExtractMethodNotSupported}"), ProjectBindingService.GetCodonPerCodeFileName(editor.FileName).Language));
					return null;
			}
		}
	}
}
