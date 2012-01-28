// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if the caller project is writable. If caller is not an IProject it tests
	/// Project.CurrentProject.
	/// </summary>
	public class WriteableProjectConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (caller is ISolutionFolderNode)
				return ProjectService.OpenSolution != null && !ProjectService.OpenSolution.ReadOnly;
			IProject project = (caller as IProject) ?? ProjectService.CurrentProject;
			return project != null && !project.ReadOnly;
		}
	}
}
