// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public static class SelectedTestsHelper
	{
		public static SelectedTests CreateSelectedTestMember()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			MockMethod methodToTest = MockMethod.CreateMockMethodWithoutAnyAttributes();
			methodToTest.FullyQualifiedName = "MyTests.MyTestClass.MyTestMethod";
			
			MockClass classToTest = methodToTest.DeclaringType as MockClass;
			classToTest.SetDotNetName("MyTests.MyTestClass");
			
			MockTestTreeView treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedMember = methodToTest;
			
			return new SelectedTests(treeView, null);
		}
	}
}
