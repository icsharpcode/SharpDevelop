// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class SimpleWebFormsHtmlReader : HtmlReader
	{
		WebFormsMarkupCharacterReader reader;
		
		public SimpleWebFormsHtmlReader(string html)
			: this(new WebFormsMarkupCharacterReader(html))
		{
		}
		
		public SimpleWebFormsHtmlReader(WebFormsMarkupCharacterReader reader)
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
