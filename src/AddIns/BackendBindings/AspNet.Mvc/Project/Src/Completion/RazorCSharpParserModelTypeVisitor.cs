// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpParserModelTypeVisitor : ParserVisitor
	{
		bool foundModelTypeName;
		
		public RazorCSharpParserModelTypeVisitor()
		{
			ModelTypeName = String.Empty;
		}
		
		public string ModelTypeName { get; private set; }
		
		public override void VisitSpan(Span span)
		{
			Console.WriteLine("Span.Kind: " + span.Kind);
			Console.WriteLine("Span.GetType(): " + span.GetType().Name);
			Console.WriteLine("Span.Content: '" + span.Content + "'");
			
			if (foundModelTypeName)
				return;
			
			if (IsModelSpan(span)) {
				VisitModelNameSpan(span.Next);
			}
		}
		
		bool IsModelSpan(Span span)
		{
			return span.Content == "model";
		}
		
		void VisitModelNameSpan(Span span)
		{
			if (span == null)
				return;
			
			string firstLineOfMarkup = GetFirstLine(span.Content);
			ModelTypeName = firstLineOfMarkup.Trim();
			foundModelTypeName = true;
		}
		
		string GetFirstLine(string text)
		{
			int endOfLineIndex = text.IndexOf('\r');
			if (endOfLineIndex > 0) {
				return text.Substring(0, endOfLineIndex);
			}
			return text;
		}
	}
}
