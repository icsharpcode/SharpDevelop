// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
			AddDefaultUsings(compilationUnit);
			AddWebViewPageClass(compilationUnit);
			return new ParseInformation(compilationUnit);
		}
		
		void AddDefaultUsings(ICompilationUnit compilationUnit)
		{
			AddUsing("System.Web.Mvc", compilationUnit);
			AddUsing("System.Web.Mvc.Ajax", compilationUnit);
			AddUsing("System.Web.Mvc.Html", compilationUnit);
			AddUsing("System.Web.Routing", compilationUnit);
		}
		
		void AddUsing(string name, ICompilationUnit compilationUnit)
		{
			DefaultUsing defaultUsing = CreateUsing(name, compilationUnit.ProjectContent);
			compilationUnit.UsingScope.Usings.Add(defaultUsing);
		}
		
		DefaultUsing CreateUsing(string namespaceName, IProjectContent projectContent)
		{
			var defaultUsing = new DefaultUsing(projectContent);
			defaultUsing.Usings.Add(namespaceName);
			return defaultUsing;
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
			IClass webViewPageBaseClass = webViewPageClass.ProjectContent.GetClass("System.Web.Mvc.WebViewPage", 1);
			if (webViewPageBaseClass != null) {
				IReturnType returnType = GetWebViewPageBaseClassReturnType(webViewPageBaseClass);
				webViewPageClass.BaseTypes.Add(returnType);
			}
		}
		
		IReturnType GetWebViewPageBaseClassReturnType(IClass webViewPageBaseClass)
		{
			var typeArguments = new List<IReturnType>();
			typeArguments.Add(new DynamicReturnType(webViewPageBaseClass.ProjectContent));
			return new ConstructedReturnType(webViewPageBaseClass.DefaultReturnType, typeArguments);
		}
		
		DomRegion GetRegionInMiddleOfWebViewPageClass()
		{
			return new DomRegion(2, 0, 2, 0);
		}
	}
}
