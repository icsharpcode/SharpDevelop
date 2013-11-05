// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using System;

namespace UnitTesting.Tests.Utils
{
	public class MockTestTreeView : ITestTreeView
	{
		TestMember selectedMember;
		TestClass selectedClass;
		TestProject selectedProject;
		string selectedNamespace;
		
		public TestMember SelectedMember {
			get { return selectedMember; }
			set { selectedMember = value; }
		}
		
		public TestClass SelectedClass {
			get { return selectedClass; }
			set { selectedClass = value; }
		}
		
		public TestProject SelectedProject {
			get { return selectedProject; }
			set { selectedProject = value; }
		}
		
		public string SelectedNamespace {
			get { return selectedNamespace; }
			set { selectedNamespace = value; }
		}
	}
}
