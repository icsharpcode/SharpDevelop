// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
			} else if (project is UnknownProject) {
				CustomNode unknownNode = new CustomNode();
				unknownNode.SetIcon("Icons.16x16.Warning");
				unknownNode.Text = StringParser.Parse(((UnknownProject)project).WarningText);
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
