// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpResolver : IResolver
	{
		NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
		
		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
		{
			ParseInformation parseInfoWithWebViewPageClass = CreateParseInformationWithWebViewPageClass(parseInfo);
			expressionResult.Region = GetRegionInMiddleOfWebViewPageClass();
			return resolver.Resolve(expressionResult, parseInfoWithWebViewPageClass, fileContent);
		}
		
		ParseInformation CreateParseInformationWithWebViewPageClass(ParseInformation parseInfo)
		{
			RazorCompilationUnit compilationUnit = RazorCompilationUnit.CreateFromParseInfo(parseInfo);
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
		
		void AddWebViewPageClass(RazorCompilationUnit compilationUnit)
		{
			DefaultClass webViewPageClass = CreateWebViewPageClass(compilationUnit);
			compilationUnit.Classes.Add(webViewPageClass);
		}
		
		DefaultClass CreateWebViewPageClass(RazorCompilationUnit compilationUnit)
		{
			var webViewPageClass = new DefaultClass(compilationUnit, "RazorWebViewPage") {
				Region = new DomRegion(1, 0, 3, 0)
			};
			IReturnType modelType = GetModelReturnType(compilationUnit);
			AddWebViewPageBaseClass(webViewPageClass, modelType);
			return webViewPageClass;
		}
		
		IReturnType GetModelReturnType(RazorCompilationUnit compilationUnit)
		{
			IClass modelType = GetClassIfTypeNameIsNotEmpty(compilationUnit.ProjectContent, compilationUnit.ModelTypeName);
			if (modelType != null) {
				return modelType.DefaultReturnType;
			}
			return new DynamicReturnType(compilationUnit.ProjectContent);
		}
		
		IClass GetClassIfTypeNameIsNotEmpty(IProjectContent projectContent, string modelTypeName)
		{
			if (!String.IsNullOrEmpty(modelTypeName)) {
				return projectContent.GetClass(modelTypeName, 0);
			}
			return null;
		}
		
		void AddWebViewPageBaseClass(DefaultClass webViewPageClass, IReturnType modelType)
		{
			IClass webViewPageBaseClass = webViewPageClass.ProjectContent.GetClass("System.Web.Mvc.WebViewPage", 1);
			if (webViewPageBaseClass != null) {
				IReturnType returnType = GetWebViewPageBaseClassReturnType(webViewPageBaseClass, modelType);
				webViewPageClass.BaseTypes.Add(returnType);
			}
		}
		
		IReturnType GetWebViewPageBaseClassReturnType(IClass webViewPageBaseClass, IReturnType modelType)
		{
			var typeArguments = new List<IReturnType>();
			typeArguments.Add(modelType);
			return new ConstructedReturnType(webViewPageBaseClass.DefaultReturnType, typeArguments);
		}
		
		DomRegion GetRegionInMiddleOfWebViewPageClass()
		{
			return new DomRegion(2, 0, 2, 0);
		}
	}
}
