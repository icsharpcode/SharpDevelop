// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class RazorHtmlFoldParser : HtmlFoldParser
	{
		public RazorHtmlFoldParser(string extension)
			: this(new RazorHtmlReaderFactory(extension))
		{
		}
		
		public RazorHtmlFoldParser(RazorHtmlReaderFactory htmlReaderFactory)
			: base(htmlReaderFactory)
		{
		}
	}
}
