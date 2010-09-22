// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
