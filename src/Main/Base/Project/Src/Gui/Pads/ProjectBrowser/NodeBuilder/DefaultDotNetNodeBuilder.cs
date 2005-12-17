// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
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
			ProjectNode projectNode = new ProjectNode(project);
			projectNode.AddTo(motherNode);
			
			if (project is MissingProject) {
				CustomNode missingNode = new CustomNode();
				missingNode.SetIcon("Icons.16x16.Warning");
				missingNode.Text = "The project file cannot be found.";
				missingNode.AddTo(projectNode);
			} else if (project is UnknownProject) {
				string ext = Path.GetExtension(project.FileName);
				if (".proj".Equals(ext, StringComparison.OrdinalIgnoreCase)
				    || ".build".Equals(ext, StringComparison.OrdinalIgnoreCase))
				{
					projectNode.OpenedImage = projectNode.ClosedImage = "Icons.16x16.XMLFileIcon";
					projectNode.Nodes.Clear();
				} else {
					CustomNode unknownNode = new CustomNode();
					unknownNode.SetIcon("Icons.16x16.Warning");
					unknownNode.Text = "No backend for project type installed.";
					unknownNode.AddTo(projectNode);
				}
			} else {
				new ReferenceFolder(project).AddTo(projectNode);
			}
			return projectNode;
		}
	}
}
