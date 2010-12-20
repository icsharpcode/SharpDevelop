// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestsPad
	{
		void UpdateToolbar();
		void BringToFront();
		void ResetTestResults();
		IProject[] GetProjects();
		TestProject GetTestProject(IProject project);		
		void CollapseAll();
	}
}
