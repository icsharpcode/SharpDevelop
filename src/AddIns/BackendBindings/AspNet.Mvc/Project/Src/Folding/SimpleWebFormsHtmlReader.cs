// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class SimpleWebFormsHtmlReader : HtmlReader
	{
		public SimpleWebFormsHtmlReader(string html)
			: this(new CharacterReader(html))
		{
		}
		
		public SimpleWebFormsHtmlReader(CharacterReader reader)
			: base(reader)
		{
		}
	}
}
