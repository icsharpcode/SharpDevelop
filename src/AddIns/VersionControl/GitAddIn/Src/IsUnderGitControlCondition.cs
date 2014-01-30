// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
