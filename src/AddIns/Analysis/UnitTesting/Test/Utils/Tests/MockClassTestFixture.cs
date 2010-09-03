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
	public class MockClassTestFixture
	{
		MockClass outerClass;
		MockClass innerClass;
		
		[Test]
		public void ClassCreatedWithExpectedFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			Assert.AreEqual(fullyQualifiedName, c.FullyQualifiedName);
		}
		
		[Test]
		public void ClassCreatedWithFullyQualifiedNameHasMatchingDotNetName()
		{
			string fullyQualifiedName = "MyNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			Assert.AreEqual(fullyQualifiedName, c.DotNetName);
		}
		
		[Test]
		public void ClassCreatedWithExpectedDotNetName()
		{
			string fullyQualifiedName = "MyNamespace.MyClass.InnerClass";
			string dotNetName = "MyNamespace.MyClass+InnerClass";
			MockClass c = new MockClass(fullyQualifiedName, dotNetName);
			Assert.AreEqual(dotNetName, c.DotNetName);
		}
		
		[Test]
		public void ClassCreatedWithNamespaceTakenFromFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MySubNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedNamespace = "MyNamespace.MySubNamespace";
			Assert.AreEqual(expectedNamespace, c.Namespace);
		}
		
		[Test]
		public void ClassCreatedWithNameTakenFromFullyQualifiedName()
		{
			string fullyQualifiedName = "MyNamespace.MySubNamespace.MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedName = "MyClass";
			Assert.AreEqual(expectedName, c.Name);
		}
		
		[Test]
		public void ClassCreatedWithNoNamespaceInFullyQualifiedNameHasNamespaceOfEmptyString()
		{
			string fullyQualifiedName = "MyClass";
			MockClass c = new MockClass(fullyQualifiedName);
			string expectedNamespace = String.Empty;
			Assert.AreEqual(expectedNamespace, c.Namespace);
		}
		
		[Test]
		public void ClassHasCompilationUnit()
		{
			MockClass c = new MockClass();
			Assert.IsNotNull(c.CompilationUnit);
		}
		
		[Test]
		public void ClassHasMockProjectContent()
		{
			MockClass c = new MockClass();
			Assert.IsNotNull(c.ProjectContent as MockProjectContent);
		}
		
		[Test]
		public void CompoundClassIsClassItself()
		{
			MockClass c = new MockClass();
			Assert.AreEqual(c, c.GetCompoundClass());
		}
		
		[Test]
		public void ClassDefaultReturnTypeGetUnderlyingClassMatchesOriginalMockClass()
		{
			MockClass c = new MockClass();
			IReturnType returnType = c.DefaultReturnType;
			Assert.AreEqual(c, returnType.GetUnderlyingClass());
		}
		
		[Test]
		public void ClassWithInnerClassHasDeclaringTypeAsOuterClass()
		{
			CreateClassWithInnerClass();
			Assert.AreEqual(outerClass, innerClass.DeclaringType);
		}
		
		void CreateClassWithInnerClass()
		{
			outerClass = new MockClass("MyTests.A");
			innerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest", outerClass);
		}
		
		[Test]
		public void InnerClassGetCompoundClassReturnsInnerClass()
		{
			CreateClassWithInnerClass();
			Assert.AreEqual(innerClass, innerClass.GetCompoundClass());
		}
		
		[Test]
		public void InnerClassAddedToOuterClassInnerClassCollection()
		{
			CreateClassWithInnerClass();
			Assert.AreEqual(innerClass, outerClass.InnerClasses[0]);
		}
		
		[Test]
		public void ClassAddedToBaseTypesBecomesBaseClass()
		{
			MockClass c = new MockClass();
			MockClass baseClass = new MockClass();
			DefaultReturnType returnType = new DefaultReturnType(baseClass);
			c.BaseTypes.Add(returnType);
			Assert.AreEqual(baseClass, c.BaseClass);
		}
		
		[Test]
		public void BaseClassPropertyReturnsClassAddedUsingAddBaseClassMethod()
		{
			MockClass c = new MockClass();
			MockClass baseClass = new MockClass();
			c.AddBaseClass(baseClass);
			Assert.AreEqual(baseClass, c.BaseClass);
		}
		
		[Test]
		public void ClassWithProjectContentHasExpectedDotNetName()
		{
			MockProjectContent projectContent = new MockProjectContent();
			string expectedName = "MyNamespace.MyTests";
			MockClass c = new MockClass(projectContent, expectedName);
			Assert.AreEqual(expectedName, c.DotNetName);
		}
		
		[Test]
		public void GetCompoundClassReturnsClassSetWithSetCompoundClass()
		{
			MockClass c = new MockClass();
			MockClass compoundClass = new MockClass();
			c.SetCompoundClass(compoundClass);
			Assert.AreEqual(compoundClass, c.GetCompoundClass());
		}
	}
}
