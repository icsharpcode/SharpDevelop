// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	public abstract class ResolveTestFixtureBase
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
