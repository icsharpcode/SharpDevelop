// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class WebFormsHtmlFoldParser : HtmlFoldParser
	{
		public WebFormsHtmlFoldParser(IHtmlReaderFactory htmlReaderFactory)
			: base(htmlReaderFactory)
		{
		}
		
		public WebFormsHtmlFoldParser()
			: this(new WebFormsHtmlReaderFactory())
		{
		}
	}
}
