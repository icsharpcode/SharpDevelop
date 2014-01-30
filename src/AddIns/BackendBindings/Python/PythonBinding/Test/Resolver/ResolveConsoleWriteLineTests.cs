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
