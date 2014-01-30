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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class HtmlTextEditorExtension : ITextEditorExtension
	{
		ITextEditorWithParseInformationFoldingFactory textEditorFactory;
		IFoldGeneratorFactory foldGeneratorFactory;
		IFoldGenerator foldGenerator;
		
		public HtmlTextEditorExtension(
			ITextEditorWithParseInformationFoldingFactory textEditorFactory,
			IFoldGeneratorFactory foldGeneratorFactory)
		{
			this.textEditorFactory = textEditorFactory;
			this.foldGeneratorFactory = foldGeneratorFactory;
		}
		
		public void Attach(ITextEditor editor)
		{
			Attach(textEditorFactory.CreateTextEditor(editor));
		}
		
		void Attach(ITextEditorWithParseInformationFolding editor)
		{
			foldGenerator = foldGeneratorFactory.CreateFoldGenerator(editor);
		}
		
		public void Detach()
		{
			foldGenerator.Dispose();
		}
	}
}
