// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Tests that the expression "Console.WriteLine" is correctly
	/// resolved.
	/// </summary>
	[TestFixture]
	public class ResolveConsoleWriteLineTests : ResolveTestsBase
	{
		MockClass systemConsoleClass;
		
		protected override ExpressionResult GetExpressionResult()
		{
			systemConsoleClass = new MockClass(projectContent, "System.Console");
			systemConsoleClass.AddMethod("WriteLine");
			projectContent.SetClassToReturnFromGetClass("Console", systemConsoleClass);
			return new ExpressionResult("Console.WriteLine", new DomRegion(2, 2), null, null);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import System\r\n" +
				"Console.WriteLine\r\n";
		}
		
		/// <summary>
		/// Gets the class name used in IProjectContent.GetClass call.
		/// </summary>
		[Test]
		public void Resolve_ExpressionIsSystemConsoleWriteLine_ProjectContentGetClassNamePassedConsole()
		{
			string className = projectContent.GetClassName;
			Assert.AreEqual("Console", className);
		}
				
		[Test]
		public void Resolve_ExpressionIsSystemConsoleWriteLine_MethodNameResolvedIsWriteLine()
		{
			MethodGroupResolveResult methodResolveResult = (MethodGroupResolveResult)resolveResult;
			string name = methodResolveResult.Name;
			string expectedName = "WriteLine";
			Assert.AreEqual(expectedName, name);
		}
	}
}
