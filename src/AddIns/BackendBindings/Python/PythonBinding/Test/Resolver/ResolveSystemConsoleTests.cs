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
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the PythonResolver correctly resolves the expression:
	/// "System.Console"
	/// </summary>
	[TestFixture]
	public class ResolveSystemConsoleTestFixture
	{
		PythonResolver resolver;
		ICSharpCode.Scripting.Tests.Utils.MockProjectContent mockProjectContent;
		ResolveResult resolveResult;
		MockClass testClass;
		ICompilationUnit compilationUnit;
		MockClass systemConsoleClass;
		ResolveResult invalidMostRecentCompilationUnitResolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new ICSharpCode.Scripting.Tests.Utils.MockProjectContent();
			
			systemConsoleClass = new MockClass(mockProjectContent, "System.Console");
			mockProjectContent.SetClassToReturnFromGetClass("System.Console", systemConsoleClass);
			
			compilationUnit = CreateCompilationUnit(mockProjectContent);
			ParseInformation parseInfo = new ParseInformation(compilationUnit);
			
			string python = GetPythonScript();
			ExpressionResult expressionResult = new ExpressionResult("System.Console", new DomRegion(3, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python);
			
			// Check that the best compilation unit is used and the resolve
			// still works.
			invalidMostRecentCompilationUnitResolveResult = resolver.Resolve(expressionResult, parseInfo, python);
		}
		
		[Test]
		public void ResolveResultExists()
		{
			Assert.IsNotNull(resolveResult);
		}
		
		[Test]
		public void IsTypeResolveResult()
		{
			Assert.IsInstanceOf(typeof(TypeResolveResult), resolveResult);
		}
		
		[Test]
		public void ResolvedClass()
		{
			TypeResolveResult typeResolveResult = resolveResult as TypeResolveResult;
			Assert.AreEqual(systemConsoleClass, typeResolveResult.ResolvedClass);
		}
		
		[Test]
		public void GetClassName()
		{
			Assert.AreEqual("System.Console", mockProjectContent.GetClassName);
		}
		
		[Test]
		public void ResolvedClassWithInvalidMostRecentCompilationUnit()
		{
			TypeResolveResult typeResolveResult = invalidMostRecentCompilationUnitResolveResult as TypeResolveResult;
			Assert.AreEqual(systemConsoleClass, typeResolveResult.ResolvedClass);
		}
		
		/// <summary>
		/// Returns the Python script that will be used for testing.
		/// </summary>
		protected virtual string GetPythonScript()
		{
			return "import System\r\n" +
					"class Test:\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tSystem.Console\r\n";
		}

		/// <summary>
		/// Creates a compilation unit with one class called Test.
		/// </summary>
		protected virtual ICompilationUnit CreateCompilationUnit(IProjectContent projectContent)
		{
			ICompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			testClass = new MockClass(projectContent, "Test");
			compilationUnit.Classes.Add(testClass);
			return compilationUnit;
		}
	}
}
