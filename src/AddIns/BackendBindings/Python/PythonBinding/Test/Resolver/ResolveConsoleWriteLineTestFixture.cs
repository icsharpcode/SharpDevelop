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
	/// Tests that the expression "Console.WriteLine" is correctly
	/// resolved.
	/// </summary>
	[TestFixture]
	public class ResolveConsoleWriteLineTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		ResolveResult resolveResult;
		ICompilationUnit compilationUnit;
		MockClass systemConsoleClass;
		MethodGroupResolveResult methodResolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			
			systemConsoleClass = new MockClass("System.Console");
			mockProjectContent.ClassToReturnFromGetClass = systemConsoleClass;
			mockProjectContent.ClassNameForGetClass = "Console";
			
			compilationUnit = new DefaultCompilationUnit(mockProjectContent);
			parseInfo.DirtyCompilationUnit = compilationUnit;
			
			string python = "import System\r\n" +
							"Console.WriteLine\r\n";
			ExpressionResult expressionResult = new ExpressionResult("Console.WriteLine", new DomRegion(2, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python);
			methodResolveResult = resolveResult as MethodGroupResolveResult;
		}
		
		[Test]
		public void ResolveResultExists()
		{
			Assert.IsNotNull(resolveResult);
		}
		
		/// <summary>
		/// Gets the class name used in IProjectContent.GetClass call.
		/// </summary>
		[Test]
		public void GetClassName()
		{
			Assert.AreEqual("Console", mockProjectContent.GetClassName);
		}
				
		[Test]
		public void MethodNameResolveIsWriteLine()
		{
			Assert.AreEqual("WriteLine", methodResolveResult.Name);
		}
	}
}
