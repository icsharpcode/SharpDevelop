// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests what happens when a test method is renamed inside an inner class.
	/// </summary>
	[TestFixture]
	public class InnerClassMethodRemovedTestFixture : InnerClassTestFixtureBase
	{
		TestClass innerTestClass;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();

			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(outerClass);
			
			// Create new compilation unit with inner class that has its method renamed.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass newOuterClass = new MockClass("MyTests.A");
			newOuterClass.ProjectContent = projectContent;
			projectContent.Classes.Add(newOuterClass);
			newOuterClass.SetCompoundClass(newOuterClass);
			newUnit.Classes.Add(newOuterClass);
			
			// Create the inner test class.
			MockClass newInnerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest");
			newInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			newInnerClass.ProjectContent = projectContent;
			newInnerClass.DeclaringType = outerClass; // Declaring type is outer class.	
			newInnerClass.SetCompoundClass(newInnerClass);
			newOuterClass.InnerClasses.Add(newInnerClass);
			
			MockMethod method = new MockMethod("FooBarRenamed");
			method.Attributes.Add(new MockAttribute("Test"));
			method.DeclaringType = newInnerClass;
			newInnerClass.Methods.Add(method);
			outerClass.InnerClasses.Add(newInnerClass);
		
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			innerTestClass = testProject.TestClasses["MyTests.A+InnerATest"];
		}
		
		[Test]
		public void NewTestMethodExists()
		{
			TestMethod method = innerTestClass.TestMethods[0];
			Assert.AreEqual("FooBarRenamed", method.Name);
		}
		
		[Test]
		public void OldTestMethodRemoved()
		{
			Assert.AreEqual(1, innerTestClass.TestMethods.Count);
		}
	}
}
