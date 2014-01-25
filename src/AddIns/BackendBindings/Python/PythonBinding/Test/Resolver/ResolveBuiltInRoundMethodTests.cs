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
