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
	/// 	public event EventHandler MyEvent;
	/// }
	/// 
	/// </summary>
	[TestFixture]
	public class ResolveClassEventTests
	{
		PythonResolverTestsHelper resolverHelper;
		IEvent myClassEvent;
		MockClass myClass;
		IProperty eventHandlerTargetProperty;
		
		void CreateClassWithOneEvent()
		{
			// Define imports.
			string code = 
				"from MyNamespace import MyClass";
			
			resolverHelper = new PythonResolverTestsHelper(code);
			myClass = resolverHelper.CreateClass("MyClass");
			myClassEvent = myClass.AddEvent("MyEvent");
			
			AddEventHandlerClass();
			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("MyNamespace.MyClass", myClass);
		}
		
		void AddEventHandlerClass()
		{
			MockClass eventHandlerClass = resolverHelper.CreateClass("EventHandler");
			eventHandlerTargetProperty = eventHandlerClass.AddProperty("Target");
			myClassEvent.ReturnType = new DefaultReturnType(eventHandlerClass);
		}
		
		[Test]
		public void Resolve_ExpressionIsForEventOnClass_MemberResolveResultResolvedTypeIsClassEvent()
		{
			CreateClassWithOneEvent();
			resolverHelper.Resolve("MyClass.MyEvent");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(myClassEvent, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionIsForSecondEventOnClass_MemberResolveResultResolvedTypeIsSecondEvent()
		{
			CreateClassWithOneEvent();
			IEvent secondEvent = myClass.AddEvent("SecondEvent");
			resolverHelper.Resolve("MyClass.SecondEvent");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(secondEvent, resolvedMember);
		}
		
		[Test]
		public void Resolve_ExpressionIsForEventHandlerTargetProperty_MemberResolveResultResolvedTypeIsEventHandlerTargetProperty()
		{
			CreateClassWithOneEvent();
			resolverHelper.Resolve("MyClass.MyEvent.Target");
			IMember resolvedMember = resolverHelper.MemberResolveResult.ResolvedMember;
			
			Assert.AreEqual(eventHandlerTargetProperty, resolvedMember);
		}
	}
}
