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
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class DefaultDotNetNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(IProject project)
		{
			return true;
		}
		
		public TreeNode AddProjectNode(TreeNode motherNode, IProject project)
		{
			ProjectNode projectNode = new ProjectNode(project);
			projectNode.InsertSorted(motherNode);
			
			if (project is MissingProject) {
				CustomNode missingNode = new CustomNode();
				missingNode.SetIcon("Icons.16x16.Warning");
				missingNode.Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ProjectFileNotFound");
				missingNode.AddTo(projectNode);
			} else if (project is ErrorProject) {
				CustomNode unknownNode = new CustomNode();
				unknownNode.SetIcon("Icons.16x16.Warning");
				unknownNode.Text = ((ErrorProject)project).Exception.Message;
				unknownNode.AddTo(projectNode);
			} else if (project is MSBuildFileProject) {
				projectNode.OpenedImage = projectNode.ClosedImage = "Icons.16x16.XMLFileIcon";
				projectNode.Nodes.Clear();
			} else {
				new ReferenceFolder(project).AddTo(projectNode);
			}
			return projectNode;
		}
	}
}
