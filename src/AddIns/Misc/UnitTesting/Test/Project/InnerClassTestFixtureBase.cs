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
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Base class for testing inner classes with TestFixture information.
	/// </summary>
	public class InnerClassTestFixtureBase
	{
		protected TestClass testClass;
		protected MockClass innerClass;
		protected TestProject testProject;
		protected MockProjectContent projectContent;
		protected MockClass outerClass;
		
		protected void InitBase()
		{			
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			outerClass = new MockClass("MyTests.A");
			outerClass.ProjectContent = projectContent;
			projectContent.Classes.Add(outerClass);
			
			// Create the inner test class.
			// Note the use of the DotNetName "MyTests.A+InnerTest".
			innerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest");
			innerClass.Attributes.Add(new MockAttribute("TestFixture"));
			innerClass.ProjectContent = projectContent;
			innerClass.DeclaringType = outerClass; // Declaring type is outer class.	

			MockMethod method = new MockMethod("FooBar");
			method.Attributes.Add(new MockAttribute("Test"));
			method.DeclaringType = innerClass;
			innerClass.Methods.Add(method);
			outerClass.InnerClasses.Add(innerClass);
			
			// Add another inner class that is not a test class.
			MockClass nonTestInnerClass = new MockClass("MyTests.A.InnerBClass");
			nonTestInnerClass.ProjectContent = projectContent;
			nonTestInnerClass.DeclaringType = outerClass; // Declaring type is outer class.	
			outerClass.InnerClasses.Add(nonTestInnerClass);

			// Add another inner class with the same name as the InnerATest.
			// This makes sure duplicate classes are not added.
			MockClass duplicateInnerClass = new MockClass("MyTests.A.InnerATest", "MyTests.A+InnerATest");
			duplicateInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			duplicateInnerClass.ProjectContent = projectContent;
			duplicateInnerClass.DeclaringType = outerClass; // Declaring type is outer class.
			outerClass.InnerClasses.Add(duplicateInnerClass);
			
			testProject = new TestProject(null, projectContent);
			if (testProject.TestClasses.Count > 0) {
				testClass = testProject.TestClasses[0];
			}
		}
	}
}
