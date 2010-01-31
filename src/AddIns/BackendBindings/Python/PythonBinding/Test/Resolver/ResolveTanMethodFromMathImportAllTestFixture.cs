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
	public class ResolveTanMethodFromMathImportAllTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("tan", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return
				"from sys import *\r\n" +
				"from math import *\r\n" +
				"from socket import *\r\n" +
				"\r\n" +
				"tan\r\n" +
				"\r\n";
		}
		
		[Test]
		public void ResolveResultIsMethodGroupResolveResult()
		{
			Assert.IsTrue(resolveResult is MethodGroupResolveResult);
		}
		
		[Test]
		public void ResolveResultMethodNameIsTan()
		{
			Assert.AreEqual("tan", MethodResolveResult.Name);
		}
		
		MethodGroupResolveResult MethodResolveResult {
			get { return (MethodGroupResolveResult)resolveResult; }
		}
		
		[Test]
		public void ResolveResultContainingTypeHasOneTanMethods()
		{
			List<IMethod> tanMethods = GetTanMethods();
			Assert.AreEqual(1, tanMethods.Count);
		}
		
		List<IMethod> GetTanMethods()
		{
			List<IMethod> methods = MethodResolveResult.ContainingType.GetMethods();
			return PythonCompletionItemsHelper.FindAllMethodsFromCollection("tan", -1, methods.ToArray());
		}
	}
}
