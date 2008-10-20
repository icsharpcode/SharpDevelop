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
	/// Tests that the inner test class is removed when its TestFixture attribute is removed.
	/// </summary>
	[TestFixture]
	public class InnerClassTestFixtureAttributeRemovedTestFixture : InnerClassTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(outerClass);
			
			// Create new compilation unit with inner class that no longer has the TestFixture attribute.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass newOuterClass = new MockClass("MyTests.A");
			newOuterClass.ProjectContent = projectContent;
			projectContent.Classes.Add(newOuterClass);
			newOuterClass.SetCompoundClass(newOuterClass);
			newUnit.Classes.Add(newOuterClass);
			
			// Create the inner test class.
			MockClass newInnerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest");
			newInnerClass.ProjectContent = projectContent;
			newInnerClass.DeclaringType = outerClass; // Declaring type is outer class.	
			newInnerClass.SetCompoundClass(newInnerClass);
			newOuterClass.InnerClasses.Add(newInnerClass);
		
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);			
		}
		
		[Test]
		public void NoTestClasses()
		{
			Assert.AreEqual(0, testProject.TestClasses.Count);
		}
		
		[Test]
		public void InnerTestClassRemoved()
		{
			Assert.IsFalse(testProject.TestClasses.Contains("MyTests.A+InnerATest"));
		}
	}
}
