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
