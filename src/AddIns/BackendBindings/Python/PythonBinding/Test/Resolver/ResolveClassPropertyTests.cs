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
	/// Tests resolving properties for the following classes:
	/// 
	/// public class MyClass
	/// {
	/// 	public MyNestedPropertyClass MyProperty { get; set; }
	/// }
	/// 
	/// public class MyNestedPropertyClass
	/// {
	/// 	public object MyNestedProperty { get; set; }
	/// }
	/// 
	/// </summary>
	[TestFixture]
	public class ResolveClassPropertyTests
	{
		PythonResolverTestsHelper resolverHelper;
		IProperty myClassProperty;
		MockClass myClass;
		IProperty nestedClassProperty;
		
		void CreateClassWithOneProperty()
		{
			resolverHelper = new PythonResolverTestsHelper();
			myClass = resolverHelper.CreateClass("MyClass");
			myClassProperty = myClass.AddProperty("MyProperty");
			
			AddNestedPropertyToExistingProperty();
			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("MyClass", myClass);
		}
		
		void AddNestedPropertyToExistingProperty()
		{
			MockClass nestedPropertyClass = resolverHelper.CreateClass("MyNestedPropertyClass");
			nestedClassProperty = nestedPropertyClass.AddProperty("MyNestedProperty");
			myClassProperty.ReturnType = new DefaultReturnType(nestedPropertyClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsForPropertyOnClassWithOneProperty_MemberResolveResultResolvedTypeIsMyClassProperty()
		{
			CreateClassWithOneProperty();
			resolverHelper.Resolve("MyClass.MyProperty");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionIsForSecondPropertyOnClassWithTwoProperties_MemberResolveResultResolvedTypeIsSecondClassProperty()
		{
			CreateClassWithOneProperty();
			myClass.InsertPropertyAtStart("ExtraProperty");
			resolverHelper.Resolve("MyClass.MyProperty");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionRefersToNestedProperty_MemberResolveResultResolvedTypeIsNestedProperty()
		{
			CreateClassWithOneProperty();
			AddNestedPropertyToExistingProperty();
			resolverHelper.Resolve("MyClass.MyProperty.MyNestedProperty");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(nestedClassProperty, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionIsForPropertyOnLocalVariable_MemberResolveResultResolvedTypeIsMyClassProperty()
		{
			CreateClassWithOneProperty();
			string code =
				"a = MyClass()\r\n" +
				"a.MyProperty";
			
			resolverHelper.Resolve("a.MyProperty", code);
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassProperty, resolvedMember);
		}
	}
}
