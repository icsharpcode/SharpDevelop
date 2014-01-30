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
		RazorCodeLanguage codeLanguage;
		
		public RazorHtmlSpans(string html, string fileExtension)
		{
			codeLanguage = RazorCodeLanguage.GetLanguageByExtension(fileExtension);
			ReadHtmlSpans(html);
		}
		
		public string CodeLanguageName {
			get { return codeLanguage.LanguageName; }
		}
		
		void ReadHtmlSpans(string html)
		{
			RazorEngineHost razorEngineHost = new RazorEngineHost(codeLanguage);
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
