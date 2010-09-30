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
