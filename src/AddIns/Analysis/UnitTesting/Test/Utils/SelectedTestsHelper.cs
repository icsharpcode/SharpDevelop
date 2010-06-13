// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public static class SelectedTestsHelper
	{
		public static SelectedTests CreateSelectedTestMethod()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			MockMethod methodToTest = MockMethod.CreateMockMethodWithoutAnyAttributes();
			methodToTest.FullyQualifiedName = "MyTests.MyTestClass.MyTestMethod";
			
			MockClass classToTest = methodToTest.DeclaringType as MockClass;
			classToTest.SetDotNetName("MyTests.MyTestClass");
			
			MockTestTreeView treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedMethod = methodToTest;
			
			return new SelectedTests(treeView, null);
		}
	}
}
