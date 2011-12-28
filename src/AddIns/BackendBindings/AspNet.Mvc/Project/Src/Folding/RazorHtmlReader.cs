// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class RazorHtmlReader : HtmlReader
	{
		RazorMarkupCharacterReader reader;
		
		public RazorHtmlReader(string html)
			: this(new RazorMarkupCharacterReader(html))
		{
		}
		
		public RazorHtmlReader(RazorMarkupCharacterReader reader)
			: base(reader)
		{
			this.reader = reader;
		}
		
		protected override bool IsHtml()
		{
			return reader.IsHtml;
		}
	}
}
