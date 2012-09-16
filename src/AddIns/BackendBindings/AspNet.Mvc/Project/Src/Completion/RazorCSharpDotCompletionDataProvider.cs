// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpDotCompletionDataProvider : DotCodeCompletionItemProvider
	{
		public override ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
		{
			NRefactoryResolver resolver = CreateResolver(editor);
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			ParseInformation parseInfoWithWebViewPageClass = CreateParseInformationWithWebViewPageClass(parseInfo);
			expressionResult.Region = GetRegionInMiddleOfWebViewPageClass();
			return resolver.Resolve(expressionResult, parseInfoWithWebViewPageClass, editor.Document.Text);
		}
		
		NRefactoryResolver CreateResolver(ITextEditor editor)
		{
			return new NRefactoryResolver(LanguageProperties.CSharp) {
				LimitMethodExtractionUntilLine = editor.Caret.Line
			};
		}
		
		ParseInformation CreateParseInformationWithWebViewPageClass(ParseInformation parseInfo)
		{
			var compilationUnit = new DefaultCompilationUnit(parseInfo.CompilationUnit.ProjectContent);
			AddWebViewPageClass(compilationUnit);
			return new ParseInformation(compilationUnit);
		}
		
		void AddWebViewPageClass(DefaultCompilationUnit compilationUnit)
		{
			DefaultClass webViewPageClass = CreateWebViewPageClass(compilationUnit);
			compilationUnit.Classes.Add(webViewPageClass);
		}
		
		DefaultClass CreateWebViewPageClass(ICompilationUnit compilationUnit)
		{
			var webViewPageClass = new DefaultClass(compilationUnit, "RazorWebViewPage") {
				Region = new DomRegion(1, 0, 3, 0)
			};
			AddWebViewPageBaseClass(webViewPageClass);
			return webViewPageClass;
		}
		
		void AddWebViewPageBaseClass(DefaultClass webViewPageClass)
		{
			IClass webViewPageBaseClass = webViewPageClass.ProjectContent.GetClass("System.Web.Mvc.WebViewPage", 0);
			if (webViewPageBaseClass != null) {
				webViewPageClass.BaseTypes.Add(webViewPageBaseClass.DefaultReturnType);
			}
		}
		
		DomRegion GetRegionInMiddleOfWebViewPageClass()
		{
			return new DomRegion(2, 0, 2, 0);
		}
	}
}
