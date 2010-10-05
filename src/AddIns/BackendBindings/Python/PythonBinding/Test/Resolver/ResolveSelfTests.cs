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
		
		[Test]
		public void Resolve_ExpressionIsSelfFollowedByMethodCall_MethodGroupResolveResultContainingTypeUnderlyingClassIsFooClass()
		{
			ResolveSelfMethodExpression();
			IClass underlyingClass = resolverHelper.MethodGroupResolveResult.ContainingType.GetUnderlyingClass();
			
			Assert.AreEqual(fooClass, underlyingClass);
		}
		
		[Test]
		public void Resolve_ClassPropertyReferencedThroughSelf_MemberResolveResultResolvedMemberIsNamePropertyOnFooClass()
		{
			string code =
				"class Foo:\r\n" +
				"    def get_name(self):\r\n" +
				"        return 'test'\r\n" +
				"    name = property(fget=get_name)\r\n" +
				"\r\n";
			
			CreateResolver(code);
			resolverHelper.Resolve("self.name");
			
			IMember member = resolverHelper.MemberResolveResult.ResolvedMember;
			IMember expectedMember = fooClass.Properties[0];
			
			Assert.AreSame(expectedMember, member);
		}
		
		[Test]
		public void Resolve_PropertyReferencedThroughSelfButOutsideClass_ReturnsNull()
		{
			string code = String.Empty;
			resolverHelper = new PythonResolverTestsHelper(code);
			resolverHelper.Resolve("self.name");
			
			ResolveResult result = resolverHelper.ResolveResult;
			
			Assert.IsNull(result);
		}
	}
}
