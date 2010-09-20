// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using System;
using ICSharpCode.Core;

namespace ICSharpCode.GitAddIn
{
	/// <summary>
	/// Description of IsUnderGitControlCondition.
	/// </summary>
	public class IsUnderGitControlCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			FileNode node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node != null) {
				return Git.IsInWorkingCopy(node.FileName);
			}
			DirectoryNode dir = ProjectBrowserPad.Instance.SelectedNode as DirectoryNode;
			if (dir != null) {
				return Git.IsInWorkingCopy(dir.Directory);
			}
			SolutionNode sol = ProjectBrowserPad.Instance.SelectedNode as SolutionNode;
			if (sol != null) {
				return Git.IsInWorkingCopy(sol.Solution.Directory);
			}
			return false;
		}
	}
}
