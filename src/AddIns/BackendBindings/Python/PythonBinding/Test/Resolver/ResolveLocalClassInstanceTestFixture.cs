// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Given code:
	/// 
	/// a = Class1()
	/// 
	/// Check that the type of "a" can be obtained by the resolver.
	/// </summary>
	[TestFixture]
	[Ignore("Disabled local variable resolution for SD 3.0")]
	public class ResolveLocalClassInstanceTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		LocalResolveResult resolveResult;
		MockClass testClass;
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			testClass = new MockClass(mockProjectContent, "Test.Test1");
			mockProjectContent.ClassesInProjectContent.Add(testClass);			
			mockProjectContent.ClassToReturnFromGetClass = testClass;
			mockProjectContent.ClassNameForGetClass = "Test.Test1";

			compilationUnit = new DefaultCompilationUnit(mockProjectContent);
			compilationUnit.FileName = @"C:\Projects\Test\test.py";
			parseInfo.SetCompilationUnit(compilationUnit);

			string python = "a = Test1()\r\n" +
							"a";
			ExpressionResult expressionResult = new ExpressionResult("a", new DomRegion(2, 1), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python) as LocalResolveResult;			
		}		
		
		[Test]
		public void GetTypeOfInstance()
		{
			string code = "a = Class1()";
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual("Class1", resolver.Resolve("a", @"C:\Projects\Test\Test.py", code));
		}

		/// <summary>
		/// Tests that the NameExpression in the resolver is reset so the second assignment
		/// does not override the first.
		/// </summary>
		[Test]
		public void DifferentTypeCreatedLast()
		{
			string code = "a = Class1()\r\n" +
						"b = Class2()";
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual("Class1", resolver.Resolve("a", @"C:\Projects\Test\Test.py", code));
		}
		
		[Test]
		public void StringAssignmentShouldNotResolve()
		{
			string code = "a = \"test\"";
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual(null, resolver.Resolve("a", @"C:\Projects\Test\Test.py", code));
		}
		
		[Test]
		public void NullCodeShouldNotResolve()
		{
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual(null, resolver.Resolve("a", @"C:\Projects\Test\Test.py", null));
		}			
		
		[Test]
		public void ResolveResultIsLocalResolveResult()
		{
			Assert.IsNotNull(resolveResult);
		}

		[Test]
		public void ResolveResultVariableName()
		{
			Assert.AreEqual(resolveResult.VariableName, "a");
		}
	}
}
