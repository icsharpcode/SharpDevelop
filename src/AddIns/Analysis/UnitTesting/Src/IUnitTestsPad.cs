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
	public interface IUnitTestsPad
	{
		void UpdateToolbar();
		void BringToFront();
		void ResetTestResults();
		IProject[] GetProjects();
		TestProject GetTestProject(IProject project);
	}
}
