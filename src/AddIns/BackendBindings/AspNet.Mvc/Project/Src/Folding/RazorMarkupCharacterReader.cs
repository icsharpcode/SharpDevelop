// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class RazorMarkupCharacterReader : CharacterReader
	{
		RazorHtmlSpans htmlSpans;
		
		public RazorMarkupCharacterReader(string html, string fileExtension)
			: base(html)
		{
			htmlSpans = new RazorHtmlSpans(html, fileExtension);
		}
		
		public bool IsHtml {
			get { return htmlSpans.IsHtml(CurrentCharacterOffset); }
		}
	}
}
