// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class WebFormsLanguageBinding : HtmlLanguageBinding
	{
		public WebFormsLanguageBinding()
			: base(
				new TextEditorWithParseInformationFoldingFactory(),
				new WebFormsFoldGeneratorFactory())
		{
		}
		
		public WebFormsLanguageBinding(
			ITextEditorWithParseInformationFoldingFactory textEditorFactory,
			IFoldGeneratorFactory foldGeneratorFactory)
			: base(textEditorFactory, foldGeneratorFactory)
		{
		}
	}
}
