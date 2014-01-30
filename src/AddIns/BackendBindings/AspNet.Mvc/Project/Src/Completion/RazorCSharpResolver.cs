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

//using System;
//using System.Collections.Generic;
//using ICSharpCode.NRefactory.TypeSystem;
//
//namespace ICSharpCode.AspNet.Mvc.Completion
//{
//	public class RazorCSharpResolver : IResolver
//	{
//		NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
//		
//		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
//		{
//			ParseInformation parseInfoWithWebViewPageClass = CreateParseInformationWithWebViewPageClass(parseInfo);
//			expressionResult.Region = GetRegionInMiddleOfWebViewPageClass();
//			return resolver.Resolve(expressionResult, parseInfoWithWebViewPageClass, fileContent);
//		}
//		
//		ParseInformation CreateParseInformationWithWebViewPageClass(ParseInformation parseInfo)
//		{
//			RazorCompilationUnit compilationUnit = RazorCompilationUnit.CreateFromParseInfo(parseInfo);
//			AddDefaultUsings(compilationUnit);
//			AddWebViewPageClass(compilationUnit);
//			return new ParseInformation(compilationUnit);
//		}
//		
//		void AddDefaultUsings(ICompilationUnit compilationUnit)
//		{
//			AddUsing("System.Web.Mvc", compilationUnit);
//			AddUsing("System.Web.Mvc.Ajax", compilationUnit);
//			AddUsing("System.Web.Mvc.Html", compilationUnit);
//			AddUsing("System.Web.Routing", compilationUnit);
//		}
//		
//		void AddUsing(string name, ICompilationUnit compilationUnit)
//		{
//			DefaultUsing defaultUsing = CreateUsing(name, compilationUnit.ProjectContent);
//			compilationUnit.UsingScope.Usings.Add(defaultUsing);
//		}
//		
//		DefaultUsing CreateUsing(string namespaceName, IProjectContent projectContent)
//		{
//			var defaultUsing = new DefaultUsing(projectContent);
//			defaultUsing.Usings.Add(namespaceName);
//			return defaultUsing;
//		}
//		
//		void AddWebViewPageClass(RazorCompilationUnit compilationUnit)
//		{
//			DefaultClass webViewPageClass = CreateWebViewPageClass(compilationUnit);
//			compilationUnit.Classes.Add(webViewPageClass);
//		}
//		
//		DefaultClass CreateWebViewPageClass(RazorCompilationUnit compilationUnit)
//		{
//			var webViewPageClass = new DefaultClass(compilationUnit, "RazorWebViewPage") {
//				Region = new DomRegion(1, 0, 3, 0)
//			};
//			IReturnType modelType = GetModelReturnType(compilationUnit);
//			AddWebViewPageBaseClass(webViewPageClass, modelType);
//			return webViewPageClass;
//		}
//		
//		IReturnType GetModelReturnType(RazorCompilationUnit compilationUnit)
//		{
//			IClass modelType = GetClassIfTypeNameIsNotEmpty(compilationUnit.ProjectContent, compilationUnit.ModelTypeName);
//			if (modelType != null) {
//				return modelType.DefaultReturnType;
//			}
//			return new DynamicReturnType(compilationUnit.ProjectContent);
//		}
//		
//		IClass GetClassIfTypeNameIsNotEmpty(IProjectContent projectContent, string modelTypeName)
//		{
//			if (!String.IsNullOrEmpty(modelTypeName)) {
//				return projectContent.GetClass(modelTypeName, 0);
//			}
//			return null;
//		}
//		
//		void AddWebViewPageBaseClass(DefaultClass webViewPageClass, IReturnType modelType)
//		{
//			IClass webViewPageBaseClass = webViewPageClass.ProjectContent.GetClass("System.Web.Mvc.WebViewPage", 1);
//			if (webViewPageBaseClass != null) {
//				IReturnType returnType = GetWebViewPageBaseClassReturnType(webViewPageBaseClass, modelType);
//				webViewPageClass.BaseTypes.Add(returnType);
//			}
//		}
//		
//		IReturnType GetWebViewPageBaseClassReturnType(IClass webViewPageBaseClass, IReturnType modelType)
//		{
//			var typeArguments = new List<IReturnType>();
//			typeArguments.Add(modelType);
//			return new ConstructedReturnType(webViewPageBaseClass.DefaultReturnType, typeArguments);
//		}
//		
//		DomRegion GetRegionInMiddleOfWebViewPageClass()
//		{
//			return new DomRegion(2, 0, 2, 0);
//		}
//	}
//}
