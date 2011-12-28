// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class RazorHtmlSpans
	{
		List<Span> spans;
		
		public RazorHtmlSpans(string html)
		{
			ReadHtmlSpans(html);
		}
		
		void ReadHtmlSpans(string html)
		{
			RazorEngineHost razorEngineHost = new RazorEngineHost(RazorCodeLanguage.GetLanguageByExtension(".cshtml"));
			RazorTemplateEngine engine = new RazorTemplateEngine(razorEngineHost);
			ParserResults results = engine.ParseTemplate(new StringReader(html));
			spans = new List<Span>(results.Document.Flatten());
			spans.RemoveAll(span => span.Kind != SpanKind.Markup);
		}
		
		public bool IsHtml(int offset)
		{
			if (offset >= 0) {
				return HtmlSpansContainOffset(offset);
			}
			return true;
		}
		
		bool HtmlSpansContainOffset(int offset)
		{
			foreach (Span span in spans) {
				if (IsInSpan(span, offset)) {
					return true;
				}
			}
			return false;
		}
		
		bool IsInSpan(Span span, int offset)
		{
			int spanOffset = span.Start.AbsoluteIndex;
			return (offset >= spanOffset) && (offset < spanOffset + span.Length);
		}
	}
}
