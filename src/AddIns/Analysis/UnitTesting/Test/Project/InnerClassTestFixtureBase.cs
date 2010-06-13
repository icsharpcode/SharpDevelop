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
		protected MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		protected void InitBase()
		{			
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			outerClass = new MockClass(projectContent, "MyTests.A");
			projectContent.Classes.Add(outerClass);
			
			// Create the inner test class.
			// Note the use of the DotNetName "MyTests.A+InnerTest".
			innerClass = new MockClass(projectContent, "MyTests.A.InnerATest", "MyTests.A+InnerATest", outerClass);
			innerClass.Attributes.Add(new MockAttribute("TestFixture"));

			MockMethod method = new MockMethod(innerClass, "FooBar");
			method.Attributes.Add(new MockAttribute("Test"));
			innerClass.Methods.Add(method);
			outerClass.InnerClasses.Add(innerClass);
			
			// Add another inner class that is not a test class.
			MockClass nonTestInnerClass = new MockClass(projectContent, "MyTests.A.InnerBClass", outerClass);
			outerClass.InnerClasses.Add(nonTestInnerClass);

			// Add another inner class with the same name as the InnerATest.
			// This makes sure duplicate classes are not added.
			MockClass duplicateInnerClass = new MockClass(projectContent, "MyTests.A.InnerATest", "MyTests.A+InnerATest", outerClass);
			duplicateInnerClass.Attributes.Add(new MockAttribute("TestFixture"));
			outerClass.InnerClasses.Add(duplicateInnerClass);
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testProject = new TestProject(null, projectContent, testFrameworks);
			if (testProject.TestClasses.Count > 0) {
				testClass = testProject.TestClasses[0];
			}
		}
	}
}
