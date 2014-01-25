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
