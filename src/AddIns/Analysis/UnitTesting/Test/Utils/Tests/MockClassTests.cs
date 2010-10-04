// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockClassTests
	{
		MockClass outerClass;
		MockClass innerClass;
		
		[Test]
		public void FullyQualifiedName_NewInstanceCreatedWithFullyQualifiedName_ReturnsFullyQualifiedName()
		{
			string expectedFullyQualifiedName = "MyNamespace.MyClass";
			MockClass c = new MockClass(expectedFullyQualifiedName);
			string fullyQualifiedName = c.FullyQualifiedName;
			Assert.AreEqual(expectedFullyQualifiedName, fullyQualifiedName);
		}
		
		[Test]
		public void DotNetName_NewInstanceCreatedWithFullyQualifiedName_ReturnsDotNetNameThatMatchesFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string dotNetName = c.DotNetName;
			Assert.AreEqual(fullyQualifiedName, dotNetName);
		}
		
		[Test]
		public void DotNetName_NewInstanceCreatedWithFullyQualifiedNameAndDotNetName_ReturnsExpectedDotNetName()
		{
			string fullyQualifiedName = "MyNamespace.MyClass.InnerClass";
			string expectedDotNetName = "MyNamespace.MyClass+InnerClass";
			MockClass c = new MockClass(fullyQualifiedName, expectedDotNetName);
			string dotNetName = c.DotNetName;
			Assert.AreEqual(expectedDotNetName, dotNetName);
		}
		
		[Test]
		public void Namespace_NewInstanceCreatedWithFullyQualifiedName_ReturnsNamespaceTakenFromFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MySubNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedNamespace = "MyNamespace.MySubNamespace";
			string actualNamespace = c.Namespace;
			Assert.AreEqual(expectedNamespace, actualNamespace);
		}
		
		[Test]
		public void Name_NewInstanceCreatedWithFullyQualifiedName_ReturnsNameTakenFromFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MySubNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedName = "MyClass";
			string name = c.Name;
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void Namesapce_NewInstanceCreatedWithFullyQualifiedNameWithNoNamespace_ReturnsEmptyStringAsNamespace()
		{
			string fullyQualifiedName = "MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedNamespace = String.Empty;
			string actualNamespace = c.Namespace;
			Assert.AreEqual(expectedNamespace, actualNamespace);
		}
		
		[Test]
		public void CompilationUnit_NewInstanceCreated_ReturnsNonNullCompilationUnit()
		{
			MockClass c = new MockClass();
			ICompilationUnit unit = c.CompilationUnit;
			Assert.IsNotNull(unit);
		}
		
		[Test]
		public void ProjectContent_NewInstanceCreated_ReturnsMockProjectContent()
		{
			MockClass c = new MockClass();
			MockProjectContent projectContent = c.ProjectContent as MockProjectContent;
			Assert.IsNotNull(projectContent);
		}
		
		[Test]
		public void CompoundClass_NewInstance_ReturnsClassItself()
		{
			MockClass c = new MockClass();
			IClass compoundClass = c.GetCompoundClass();
			Assert.AreEqual(c, compoundClass);
		}
		
		[Test]
		public void DefaultReturnTypeGetUnderlyClass_NewInstance_ReturnsOriginalMockClass()
		{
			MockClass c = new MockClass();
			IReturnType returnType = c.DefaultReturnType;
			IClass underlyingClass = returnType.GetUnderlyingClass();
			Assert.AreEqual(c, underlyingClass);
		}
		
		[Test]
		public void DeclaringType_NewInstanceWithInnerClass_InnerClassHasOuterClassAsDeclaringType()
		{
			CreateClassWithInnerClass();
			IClass declaringType = innerClass.DeclaringType;
			Assert.AreEqual(outerClass, declaringType);
		}
		
		void CreateClassWithInnerClass()
		{
			outerClass = new MockClass("MyTests.A");
			innerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest", outerClass);
		}
		
		[Test]
		public void GetCompoundClass_NewInstanceWithInnerClass_InnerClassReturnsInnerClassAsCompoundClass()
		{
			CreateClassWithInnerClass();
			IClass compoundClass = innerClass.GetCompoundClass();
			Assert.AreEqual(innerClass, compoundClass);
		}
		
		[Test]
		public void InnerClasses_NewInstanceCreatedWithInnerClass_FirstClassInCollectionIsInnerClass()
		{
			CreateClassWithInnerClass();
			IClass firstInnerClass = outerClass.InnerClasses[0];
			Assert.AreEqual(innerClass, firstInnerClass);
		}
		
		[Test]
		public void BaseClass_ClassAddedToBaseTypes_ClassAddedToBaseTypesBecomesBaseClass()
		{
			MockClass c = new MockClass();
			MockClass baseClass = new MockClass();
			DefaultReturnType returnType = new DefaultReturnType(baseClass);
			c.BaseTypes.Add(returnType);
			IClass actualBaseClass = c.BaseClass;
			Assert.AreEqual(baseClass, actualBaseClass);
		}
		
		[Test]
		public void BaseClass_AddBaseClassMethodCalled_ReturnsClassAddedUsingAddBaseClassMethod()
		{
			MockClass c = new MockClass();
			MockClass baseClass = new MockClass();
			c.AddBaseClass(baseClass);
			IClass actualBaseClass = c.BaseClass;
			Assert.AreEqual(baseClass, actualBaseClass);
		}
		
		[Test]
		public void DotNetName_NewInstanceCreatedWithProjectContent_ReturnsFullyQualifiedNameAsDotNetName()
		{
			MockProjectContent projectContent = new MockProjectContent();
			string expectedName = "MyNamespace.MyTests";
			MockClass c = new MockClass(projectContent, expectedName);
			string dotNetName = c.DotNetName;
			Assert.AreEqual(expectedName, dotNetName);
		}
		
		[Test]
		public void GetCompoundClass_SetCompoundClassMethodCalled_ReturnsClassPassedToSetCompoundClass()
		{
			MockClass c = new MockClass();
			MockClass compoundClass = new MockClass();
			c.SetCompoundClass(compoundClass);
			IClass actualCompoundClass = c.GetCompoundClass();
			Assert.AreEqual(compoundClass, actualCompoundClass);
		}
		
		[Test]
		public void AddProperty_PassedPropertyName_AddsPropertyToClass()
		{
			MockClass c = new MockClass();
			c.AddProperty("MyProperty");
			
			IProperty property = c.Properties[0];
			string name = property.Name;
			
			Assert.AreEqual("MyProperty", name);
		}
		
		[Test]
		public void InsertPropertyAtStart_PassedPropertyName_AddsPropertyAsFirstProperty()
		{
			MockClass c = new MockClass();
			c.AddProperty("SecondProperty");
			c.InsertPropertyAtStart("FirstProperty");
			
			IProperty property = c.Properties[0];
			string name = property.Name;
			
			Assert.AreEqual("FirstProperty", name);			
		}
		
		[Test]
		public void AddProperty_PassedPropertyName_ReturnsPropertyWithExpectedName()
		{
			MockClass c = new MockClass();
			IProperty property = c.AddProperty("MyProperty");			
			string name = property.Name;
			
			Assert.AreEqual("MyProperty", name);
		}
		
		[Test]
		public void AddEvent_PassedEventName_AddsEventToClass()
		{
			MockClass c = new MockClass();
			c.AddEvent("MyEvent");
			
			IEvent myEvent = c.Events[0];
			string name = myEvent.Name;
			
			Assert.AreEqual("MyEvent", name);
		}
		
		[Test]
		public void AddEvent_PassedEventName_ReturnsEventWithExpectedName()
		{
			MockClass c = new MockClass();
			IEvent myEvent = c.AddEvent("MyEvent");			
			string name = myEvent.Name;
			
			Assert.AreEqual("MyEvent", name);
		}
		
		[Test]
		public void AddField_PassedFieldName_AddsFieldToClass()
		{
			MockClass c = new MockClass();
			c.AddField("MyField");
			
			IField myField = c.Fields[0];
			string name = myField.Name;
			
			Assert.AreEqual("MyField", name);
		}
		
		[Test]
		public void AddField_PassedFieldName_ReturnsFieldWithExpectedName()
		{
			MockClass c = new MockClass();
			IField myField = c.AddField("MyField");			
			string name = myField.Name;
			
			Assert.AreEqual("MyField", name);
		}
		
		[Test]
		public void AddMethod_PassedName_AddsMethodToClass()
		{
			MockClass c = new MockClass();
			c.AddMethod("MyMethod");
			
			IMethod myMethod = c.Methods[0];
			string name = myMethod.Name;
			
			Assert.AreEqual("MyMethod", name);
		}
		
		[Test]
		public void AddMethod_PassedMethodName_ReturnsMethodWithExpectedName()
		{
			MockClass c = new MockClass();
			IMethod myMethod = c.AddMethod("MyMethod");			
			string name = myMethod.Name;
			
			Assert.AreEqual("MyMethod", name);
		}
	}
}
