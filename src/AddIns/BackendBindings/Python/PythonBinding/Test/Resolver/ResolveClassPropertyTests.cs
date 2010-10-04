// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
