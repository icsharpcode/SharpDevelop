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

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class NodeBuilders
	{
		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static TreeNode AddProjectNode(TreeNode motherNode, IProject project)
		{
			IProjectNodeBuilder   projectNodeBuilder = null;
			foreach (IProjectNodeBuilder nodeBuilder in AddInTree.BuildItems<IProjectNodeBuilder>("/SharpDevelop/Views/ProjectBrowser/NodeBuilders", null, true)) {
				if (nodeBuilder.CanBuildProjectTree(project)) {
					projectNodeBuilder = nodeBuilder;
					break;
				}
			}
			if (projectNodeBuilder != null) {
				return projectNodeBuilder.AddProjectNode(motherNode, project);
			}

			throw new NotImplementedException("can't create node builder for project type " + project.Language);
		}
	}
}
