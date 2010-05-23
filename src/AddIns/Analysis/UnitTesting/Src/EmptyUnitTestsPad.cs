// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
