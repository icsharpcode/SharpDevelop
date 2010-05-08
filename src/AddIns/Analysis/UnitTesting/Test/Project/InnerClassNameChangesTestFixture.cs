// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject is correctly updated after the inner class name changes.
	/// </summary>
	[TestFixture]
	public class InnerClassNameChangesTestFixture : InnerClassTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();

			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(outerClass);
			
			// Create new compilation unit with extra class.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass newOuterClass = new MockClass("MyTests.A");
			newOuterClass.ProjectContent = projectContent;
			projectContent.Classes.Add(newOuterClass);
			newOuterClass.SetCompoundClass(newOuterClass);
			newUnit.Classes.Add(newOuterClass);
			
			// Create the inner test class.
			// Note the use of the DotNetName "MyTests.A+InnerTest".
			MockClass newInnerClass = new MockClass("MyTests.A.InnerATestMod", "MyTests.A+InnerATestMod");
			newInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			newInnerClass.ProjectContent = projectContent;
			newInnerClass.DeclaringType = outerClass; // Declaring type is outer class.	
			newOuterClass.InnerClasses.Add(newInnerClass);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
		}

		[Test]
		public void NewInnerClassAdded()
		{
			Assert.IsTrue(testProject.TestClasses.Contains("MyTests.A+InnerATestMod"));
		}
		
		[Test]
		public void OldInnerClassRemoved()
		{
			Assert.IsFalse(testProject.TestClasses.Contains("MyTests.A+InnerATest"));
		}
		
		[Test]
		public void OneTestClassRemain()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
	}
}
