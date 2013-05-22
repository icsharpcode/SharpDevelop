// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
