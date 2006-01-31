// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.MbUnitPad;
using System;

namespace ICSharpCode.CodeCoverage
{
	public class RunMbUnitPadTestsWithCodeCoverageCommand : AbstractRunTestsWithCodeCoverageCommand
	{
        protected override string GetTestAssemblyFileName()
        {
        	return MbUnitPadContent.Instance.SelectedAssemblyFileName;
        }
        
        protected override IProject GetProject()
        {
        	return GetSelectedProject(GetTestAssemblyFileName());
        }
        
        IProject GetSelectedProject(string outputAssemblyFileName)
		{
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				if (FileUtility.IsEqualFileName(outputAssemblyFileName, project.OutputAssemblyFullPath)) {
					return project;
				}
			}
			return null;
		}
	}
}
