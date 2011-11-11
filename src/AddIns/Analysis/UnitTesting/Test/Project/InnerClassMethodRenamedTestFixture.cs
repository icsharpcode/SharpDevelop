// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
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
			MockClass newOuterClass = new MockClass(projectContent, "MyTests.A");
			projectContent.Classes.Add(newOuterClass);
			newUnit.Classes.Add(newOuterClass);
			
			// Create the inner test class.
			MockClass newInnerClass = new MockClass(projectContent, "MyTests.A.InnerATest", outerClass);
			newInnerClass.SetDotNetName("MyTests.A+InnerATest");
			newInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			newOuterClass.InnerClasses.Add(newInnerClass);
			
			MockMethod method = new MockMethod(newInnerClass, "FooBarRenamed");
			method.Attributes.Add(new MockAttribute("Test"));
			newInnerClass.Methods.Add(method);
			outerClass.InnerClasses.Add(newInnerClass);
			
			MockClass innerClassInInnerClass = new MockClass(projectContent, "MyTests.A.InnerATest.InnerInnerTest", innerClass);
			innerClassInInnerClass.SetDotNetName("MyTests.A+InnerATest+InnerInnerTest");
			innerClassInInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			newInnerClass.InnerClasses.Add(innerClassInInnerClass);
		
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			innerTestClass = testProject.TestClasses["MyTests.A+InnerATest"];
		}
		
		[Test]
		public void NewTestMethodExists()
		{
			TestMember method = innerTestClass.TestMembers[0];
			Assert.AreEqual("FooBarRenamed", method.Name);
		}
		
		[Test]
		public void OldTestMethodRemoved()
		{
			Assert.AreEqual(1, innerTestClass.TestMembers.Count);
		}
		
		[Test]
		public void NewTestClassExists() {
			CollectionAssert.Contains(testProject.TestClasses.Select(x => x.QualifiedName).ToList(), "MyTests.A+InnerATest+InnerInnerTest");
		}
	}
}
