// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveLocalVariableMethodTests
	{
		PythonResolverTestsHelper resolverHelper;
		MockClass myClass;
		
		void CreateClassWithOneProperty()
		{
			resolverHelper = new PythonResolverTestsHelper();
			myClass = resolverHelper.CreateClass("MyClass");
			myClass.AddMethod("MyMethod");
			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("MyClass", myClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsForMethodOnLocalVariable_MethodGroupResolveResultNameIsMethodName()
		{
			CreateClassWithOneProperty();
			string code =
				"a = MyClass()\r\n" +
				"a.MyMethod";
			
			resolverHelper.Resolve("a.MyMethod", code);
			string methodName = resolverHelper.MethodGroupResolveResult.Name;
			string expectedMethodName = "MyMethod";
			
			Assert.AreEqual(expectedMethodName, methodName);
		}
		
		[Test]
		public void Resolve_ExpressionIsForMethodOnLocalVariable_MethodGroupResolveResultContainingTypeUnderlyingClassIsMyClass()
		{
			CreateClassWithOneProperty();
			string code =
				"a = MyClass()\r\n" +
				"a.MyMethod";
			
			resolverHelper.Resolve("a.MyMethod", code);
			IClass c = resolverHelper.MethodGroupResolveResult.ContainingType.GetUnderlyingClass();
			
			Assert.AreEqual(myClass, c);
		}
	}
}
