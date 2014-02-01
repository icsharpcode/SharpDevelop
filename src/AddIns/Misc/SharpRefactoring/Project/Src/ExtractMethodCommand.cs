// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
