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
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using UnitTesting.Tests.Utils;
using ScriptingUtils = ICSharpCode.Scripting.Tests.Utils;

namespace PythonBinding.Tests.Utils
{
	public class PythonResolverTestsHelper
	{
		public ScriptingUtils.MockProjectContent ProjectContent;
		public	DefaultCompilationUnit CompilationUnit;
		public ParseInformation ParseInfo;
		public PythonResolver Resolver;
		public ResolveResult ResolveResult;
		
		public PythonResolverTestsHelper(string code)
		{
			ProjectContent = new ScriptingUtils.MockProjectContent();
			PythonParser parser = new PythonParser();
			string fileName = @"test.py";
			CompilationUnit = parser.Parse(ProjectContent, fileName, code) as DefaultCompilationUnit;
			
			ParseInfo = new ParseInformation(CompilationUnit);			
			Resolver = new PythonResolver();
		}
		
		public PythonResolverTestsHelper()
			: this(String.Empty)
		{
		}
		
		public ResolveResult Resolve(string expression)
		{
			ExpressionResult expressionResult = new ExpressionResult(expression);
			PythonResolverContext context = new PythonResolverContext(ParseInfo, expressionResult, String.Empty);
			ResolveResult = Resolver.Resolve(context);
			return ResolveResult;
		}
		
		public ResolveResult Resolve(string expression, string code)
		{
			ExpressionResult expressionResult = new ExpressionResult(expression);
			return Resolve(expressionResult, code);
		}
		
		public ResolveResult Resolve(ExpressionResult expressionResult, string code)
		{
			ResolveResult = Resolver.Resolve(expressionResult, ParseInfo, code);
			return ResolveResult;
		}
		
		public MemberResolveResult MemberResolveResult {
			get { return ResolveResult as MemberResolveResult; }
		}
		
		public LocalResolveResult LocalResolveResult {
			get { return ResolveResult as LocalResolveResult; }
		}
		
		public MethodGroupResolveResult MethodGroupResolveResult {
			get { return ResolveResult as MethodGroupResolveResult; }
		}
		
		public MockClass CreateClass(string name)
		{
			return new MockClass(ProjectContent, name);
		}
	}
}
