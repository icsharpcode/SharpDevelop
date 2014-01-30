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
	/// <summary>
	/// Tests resolving events for the following classes:
	/// 
	/// public class MyClass
	/// {
	/// 	public int MyField
	/// }
	/// 
	/// public class MyClassWithFields
	/// {
	/// 	public int MyField = 0;
	/// }
	/// 
	/// </summary>
	[TestFixture]
	public class ResolveClassFieldTests
	{
		PythonResolverTestsHelper resolverHelper;
		IField myClassField;
		MockClass myClass;
		
		void CreateClassWithOneEvent()
		{
			resolverHelper = new PythonResolverTestsHelper();
			myClass = resolverHelper.CreateClass("MyClass");
			myClassField = myClass.AddField("MyField");
			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("MyClass", myClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsForFieldOnClass_MemberResolveResultResolvedTypeIsClassField()
		{
			CreateClassWithOneEvent();
			resolverHelper.Resolve("MyClass.MyField");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassField, resolvedMember);
		}
	}
}
