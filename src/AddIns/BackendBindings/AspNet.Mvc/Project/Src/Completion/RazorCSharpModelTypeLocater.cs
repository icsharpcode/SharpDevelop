// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Web.Razor;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpModelTypeLocater
	{
		public RazorCSharpModelTypeLocater(ITextBuffer textBuffer)
		{
			ParserResults results = ParseTemplate(textBuffer);
			ModelTypeName = GetModelTypeName(results);
		}
		
		ParserResults ParseTemplate(ITextBuffer textBuffer)
		{
			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
			var engine = new RazorTemplateEngine(host);
			return engine.ParseTemplate(textBuffer.CreateReader());
		}
		
		string GetModelTypeName(ParserResults results)
		{
			var visitor = new RazorCSharpParserModelTypeVisitor();
			results.Document.Accept(visitor);
			return visitor.ModelTypeName;
		}
		
		public string ModelTypeName { get; private set; }
	}
}
