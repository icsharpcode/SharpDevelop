// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveExitMethodFromSysImportExitTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("exit", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from sys import exit\r\n" +
				"exit\r\n" +
				"\r\n";
		}
			
		[Test]
		public void ResolveResultIsMethodGroupResolveResult()
		{
			Assert.IsTrue(resolveResult is MethodGroupResolveResult);
		}
		
		[Test]
		public void ResolveResultMethodNameIsExit()
		{
			Assert.AreEqual("exit", MethodResolveResult.Name);
		}
		
		MethodGroupResolveResult MethodResolveResult {
			get { return (MethodGroupResolveResult)resolveResult; }
		}
		
		[Test]
		public void ResolveResultContainingTypeHasTwoExitMethods()
		{
			List<IMethod> exitMethods = GetExitMethods();
			Assert.AreEqual(2, exitMethods.Count);
		}
		
		List<IMethod> GetExitMethods()
		{
			List<IMethod> methods = MethodResolveResult.ContainingType.GetMethods();
			return PythonCompletionItemsHelper.FindAllMethodsFromCollection("exit", -1, methods.ToArray());
		}
	}
}
