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
	/// Tests the PythonResolver correctly resolves the expression:
	/// "System.Console"
	/// </summary>
	[TestFixture]
	public class ResolveSystemConsoleTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		ResolveResult resolveResult;
		MockClass testClass;
		ICompilationUnit compilationUnit;
		MockClass systemConsoleClass;
		ResolveResult invalidMostRecentCompilationUnitResolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			
			systemConsoleClass = new MockClass(mockProjectContent, "System.Console");
			mockProjectContent.ClassToReturnFromGetClass = systemConsoleClass;
			
			compilationUnit = CreateCompilationUnit(mockProjectContent);
			parseInfo.SetCompilationUnit(compilationUnit);
			
			string python = GetPythonScript();
			ExpressionResult expressionResult = new ExpressionResult("System.Console", new DomRegion(3, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python);
	
			// Here the most recent compilation unit is modified so it has no
			// class information. The best compilation unit (i.e. the valid one) 
			// should be used instead by the python resolver.
			parseInfo.SetCompilationUnit(compilationUnit);
			parseInfo.SetCompilationUnit(new DefaultCompilationUnit(mockProjectContent) { ErrorsDuringCompile = true });
			
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
		public void IsGetClassCalled()
		{
			Assert.IsTrue(mockProjectContent.GetClassCalled);
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
			testClass = new MockClass(compilationUnit, "Test");
			compilationUnit.Classes.Add(testClass);
			return compilationUnit;
		}
	}
}
