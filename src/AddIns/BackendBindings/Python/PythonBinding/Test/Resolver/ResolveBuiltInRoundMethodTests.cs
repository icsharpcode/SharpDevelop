// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class ResolveBuiltInRoundMethodTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("round", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"round\r\n" +
				"\r\n";
		}
			
		[Test]
		public void ResolveResultIsMethodGroupResolveResult()
		{
			Assert.IsTrue(resolveResult is MethodGroupResolveResult);
		}
		
		[Test]
		public void ResolveResultMethodNameIsRound()
		{
			Assert.AreEqual("round", MethodResolveResult.Name);
		}
		
		MethodGroupResolveResult MethodResolveResult {
			get { return (MethodGroupResolveResult)resolveResult; }
		}
		
		[Test]
		public void ResolveResultContainingTypeHasFourRoundMethods()
		{
			List<IMethod> exitMethods = GetRoundMethods();
			Assert.AreEqual(4, exitMethods.Count);
		}
		
		List<IMethod> GetRoundMethods()
		{
			return GetRoundMethods(-1);
		}
		
		List<IMethod> GetRoundMethods(int parameterCount)
		{
			List<IMethod> methods = MethodResolveResult.ContainingType.GetMethods();
			return PythonCompletionItemsHelper.FindAllMethodsFromCollection("round", parameterCount, methods.ToArray());
		}
		
		[Test]
		public void BothRoundMethodsArePublic()
		{
			foreach (IMethod method in GetRoundMethods()) {
				Assert.IsTrue(method.IsPublic);
			}
		}
		
		[Test]
		public void BothRoundMethodsHaveClassWithNameOfSys()
		{
			foreach (IMethod method in GetRoundMethods()) {
				Assert.AreEqual("__builtin__", method.DeclaringType.Name);
			}
		}
		
		[Test]
		public void ThreeRoundMethodsHaveTwoParameters()
		{
			int parameterCount = 2;
			Assert.AreEqual(3, GetRoundMethods(parameterCount).Count);
		}
		
		[Test]
		public void RoundMethodParameterNameIsNumber()
		{
			IParameter parameter = GetFirstRoundMethodParameter();
			Assert.AreEqual("number", parameter.Name);
		}
		
		IParameter GetFirstRoundMethodParameter()
		{
			int parameterCount = 1;
			List<IMethod> methods = GetRoundMethods(parameterCount);
			IMethod method = methods[0];
			return method.Parameters[0];
		}
		
		[Test]
		public void RoundMethodParameterReturnTypeIsDouble()
		{
			IParameter parameter = GetFirstRoundMethodParameter();
			Assert.AreEqual("Double", parameter.ReturnType.Name);
		}
		
		[Test]
		public void RoundMethodParameterConvertedToStringUsingAmbienceReturnsDoubleNumberString()
		{
			IAmbience ambience = new CSharpAmbience();
			string text = ambience.Convert(GetFirstRoundMethodParameter());
			Assert.AreEqual("double number", text);
		}
	}
}
