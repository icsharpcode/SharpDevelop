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
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	public abstract class ResolveTestsBase
	{
		protected ICompilationUnit compilationUnit;
		protected MockProjectContent projectContent;
		protected PythonResolver resolver;
		protected ResolveResult resolveResult;
		protected ParseInformation parseInfo;
		protected ExpressionResult expressionResult;
		
		[SetUp]
		public void InitBase()
		{
			projectContent = new MockProjectContent();
			PythonParser parser = new PythonParser();
			string fileName = @"C:\projects\test\test.py";
			compilationUnit = parser.Parse(projectContent, fileName, GetPythonScript());
			parseInfo = new ParseInformation(compilationUnit);
			
			resolver = new PythonResolver();
			
			expressionResult = GetExpressionResult();
			resolveResult = resolver.Resolve(expressionResult, parseInfo, GetPythonScript());
		}
		
		protected abstract ExpressionResult GetExpressionResult();
		
		protected abstract string GetPythonScript();
	}
}
