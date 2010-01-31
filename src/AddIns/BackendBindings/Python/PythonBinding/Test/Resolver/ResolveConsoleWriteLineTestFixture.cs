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
	public class ResolveConsoleWriteLineTestFixture : ResolveTestFixtureBase
	{
		MockClass systemConsoleClass;
		
		protected override ExpressionResult GetExpressionResult()
		{
			systemConsoleClass = new MockClass(projectContent, "System.Console");
			projectContent.ClassToReturnFromGetClass = systemConsoleClass;
			projectContent.ClassNameForGetClass = "Console";
			return new ExpressionResult("Console.WriteLine", new DomRegion(2, 2), null, null);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import System\r\n" +
				"Console.WriteLine\r\n";
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
			Assert.AreEqual("Console", projectContent.GetClassName);
		}
				
		[Test]
		public void MethodNameResolveIsWriteLine()
		{
			MethodGroupResolveResult methodResolveResult = (MethodGroupResolveResult)resolveResult;
			Assert.AreEqual("WriteLine", methodResolveResult.Name);
		}
	}
}
