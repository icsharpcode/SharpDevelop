// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class EmptyUnitTestsPad : IUnitTestsPad
	{
		public void UpdateToolbar()
		{
		}
		
		public void BringToFront()
		{
		}
		
		public void ResetTestResults()
		{
		}
		
		public IProject[] GetProjects()
		{
			return new IProject[0];
		}
		
		public TestProject GetTestProject(IProject project)
		{
			return null;
		}
		
		public void CollapseAll() { }
	}
}
