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
	public class ResolveSelfTests
	{
		IClass fooClass;
		PythonResolverTestsHelper resolverHelper;
		
		void CreateResolver(string code)
		{
			resolverHelper = new PythonResolverTestsHelper(code);
			fooClass = resolverHelper.CompilationUnit.Classes[0];
		}
		
		[Test]
		public void Resolve_ExpressionIsSelf_ResolveResultResolvedTypeUnderlyingClassReturnsFooClass()
		{
			ResolveSelfExpression();
			IClass underlyingClass = resolverHelper.ResolveResult.ResolvedType.GetUnderlyingClass();
			
			Assert.AreEqual(fooClass, underlyingClass);
		}
		
		void ResolveSelfExpression()
		{
			string code =
				"class Foo:\r\n" +
				"    def bar(self):\r\n" +
				"        self\r\n" +
				"\r\n";
			
			CreateResolver(code);
			resolverHelper.Resolve("self");
		}
		
		[Test]
		public void Resolve_ExpressionIsSelf_ResolveResultCallingClassReturnsFooClass()
		{
			ResolveSelfExpression();
			IClass underlyingClass = resolverHelper.ResolveResult.CallingClass;
			
			Assert.AreEqual(fooClass, underlyingClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsSelfFollowedByMethodCall_MethodGroupResolveResultNameIsMethodCalled()
		{
			ResolveSelfMethodExpression();
			string methodName = resolverHelper.MethodGroupResolveResult.Name;
			
			Assert.AreEqual("bar", methodName);
		}
		
		void ResolveSelfMethodExpression()
		{
			string code =
				"class Foo:\r\n" +
				"    def bar(self):\r\n" +
				"        return 0\r\n" +
				"\r\n";
			
			CreateResolver(code);
			resolverHelper.Resolve("self.bar");
		}
		
		[Test]
		public void Resolve_ExpressionIsSelfFollowedByMethodCall_MethodGroupResolveResultContainingTypeUnderlyingClassIsFooClass()
		{
			ResolveSelfMethodExpression();
			IClass underlyingClass = resolverHelper.MethodGroupResolveResult.ContainingType.GetUnderlyingClass();
			
			Assert.AreEqual(fooClass, underlyingClass);
		}
	}
}
