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
