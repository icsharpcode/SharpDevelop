// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.Core;
using System.Windows.Forms;

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
			AbstractProjectBrowserTreeNode projectNode = new ProjectNode(project);
			projectNode.AddTo(motherNode);
			
			if (project is MissingProject) {
				CustomNode missingNode = new CustomNode();
				missingNode.SetIcon("Icons.16x16.Warning");
				missingNode.Text = "The project file cannot be found.";
				missingNode.AddTo(projectNode);
				return missingNode;
			} else if (project is UnknownProject) {
				CustomNode unknownNode = new CustomNode();
				unknownNode.SetIcon("Icons.16x16.Warning");
				unknownNode.Text = "No backend for project type installed.";
				unknownNode.AddTo(projectNode);
				return unknownNode;
			}
			
			new ReferenceFolder(project).AddTo(projectNode);
			return projectNode;
		}
		
	
		
	}
}
