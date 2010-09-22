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
	public class ResolveSysModuleExitMethodTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("sys.exit", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import sys\r\n" +
				"sys.exit\r\n" +
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
			return GetExitMethods(-1);
		}
		
		List<IMethod> GetExitMethods(int parameterCount)
		{
			List<IMethod> methods = MethodResolveResult.ContainingType.GetMethods();
			return PythonCompletionItemsHelper.FindAllMethodsFromCollection("exit", parameterCount, methods.ToArray());
		}
		
		[Test]
		public void BothExitMethodsArePublic()
		{
			foreach (IMethod method in GetExitMethods()) {
				Assert.IsTrue(method.IsPublic);
			}
		}
		
		[Test]
		public void BothExitMethodsHaveClassWithNameOfSys()
		{
			foreach (IMethod method in GetExitMethods()) {
				Assert.AreEqual("sys", method.DeclaringType.Name);
			}
		}
		
		[Test]
		public void OneExitMethodHasOneParameter()
		{
			int parameterCount = 1;
			Assert.AreEqual(1, GetExitMethods(parameterCount).Count);
		}
		
		[Test]
		public void ExitMethodParameterNameIsCode()
		{
			IParameter parameter = GetFirstExitMethodParameter();
			Assert.AreEqual("code", parameter.Name);
		}
		
		IParameter GetFirstExitMethodParameter()
		{
			int parameterCount = 1;
			List<IMethod> methods = GetExitMethods(parameterCount);
			IMethod method = methods[0];
			return method.Parameters[0];
		}
		
		[Test]
		public void ExitMethodParameterReturnTypeIsObject()
		{
			IParameter parameter = GetFirstExitMethodParameter();
			Assert.AreEqual("Object", parameter.ReturnType.Name);
		}
		
		[Test]
		public void ExitMethodParameterConvertedToStringUsingAmbienceReturnsObjectCodeString()
		{
			IAmbience ambience = new CSharpAmbience();
			string text = ambience.Convert(GetFirstExitMethodParameter());
			Assert.AreEqual("object code", text);
		}
		
		[Test]
		public void ExitMethodReturnTypeConvertedToStringUsingAmbienceReturnsVoid()
		{
			IAmbience ambience = new CSharpAmbience();
			List<IMethod> methods = GetExitMethods();
			IReturnType returnType = methods[0].ReturnType;
			string text = ambience.Convert(returnType);
			Assert.AreEqual("void", text);
		}
		
		[Test]
		public void MethodGroupContainingTypeHasTwoExitMethods()
		{
			IReturnType returnType = MethodResolveResult.ContainingType;
			List<IMethod> methods = PythonCompletionItemsHelper.FindAllMethodsFromCollection("exit", returnType.GetMethods());
			Assert.AreEqual(2, methods.Count);
		}
	}
}
