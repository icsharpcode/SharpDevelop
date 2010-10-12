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
	public class ResolveMethodReturnValueTests
	{
		PythonResolverTestsHelper resolverHelper;
		MockClass myClass;
		DefaultMethod myMethod;
		
		void CreateResolver()
		{
			resolverHelper = new PythonResolverTestsHelper();
			myClass = resolverHelper.CreateClass("MyClass");
			myMethod = myClass.AddMethod("MyMethod");
			myMethod.ReturnType = new DefaultReturnType(myClass);
			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("MyClass", myClass);
		}
		
		[Test]
		public void Resolve_ExpressionToResolveIsMethodCallWithNoParameters_ReturnsMemberResolveResultWithResolvedMemberAsMethod()
		{
			CreateResolver();
			string code = "MyClass.MyMethod()";
			resolverHelper.Resolve(code);
			
			MemberResolveResult result = resolverHelper.MemberResolveResult;
			IMember resolvedMember = result.ResolvedMember;
			
			Assert.AreEqual(myMethod, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionToResolveIsMethodCallWithNoParameters_ReturnsMemberResolveResultWithMethodReturnValueAsResolvedType()
		{
			CreateResolver();
			string code = "MyClass.MyMethod()";
			resolverHelper.Resolve(code);
			
			MemberResolveResult result = resolverHelper.MemberResolveResult;
			IReturnType resolvedType = result.ResolvedType;
			IReturnType expectedResolvedType = myMethod.ReturnType;
			
			Assert.AreEqual(expectedResolvedType, resolvedType);
		}
		
		[Test]
		public void Resolve_ExpressionHasCloseParenthesisButNoOpenParenthesis_NoArgumentOutOfRangeExceptionThrownAndResolveResultIsNull()
		{
			CreateResolver();
			string code = "MyClass.MyMethod)";
			resolverHelper.Resolve(code);
			
			Assert.IsNull(resolverHelper.ResolveResult);
		}
	}
}
