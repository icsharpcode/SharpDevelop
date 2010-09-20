// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class WixProjectNodeBuilder : IProjectNodeBuilder
	{
		public WixProjectNodeBuilder()
		{
		}
		
		public bool CanBuildProjectTree(IProject project)
		{
			return project is WixProject;
		}
		
		/// <summary>
		/// Adds a WixProjectNode to the tree. This node will have a
		/// References node, a Wix Extensions node and a Wix Libraries node.
		/// </summary>
		public TreeNode AddProjectNode(TreeNode motherNode, IProject project)
		{
			ProjectNode projectNode = new ProjectNode(project);
			projectNode.AddTo(motherNode);
			
			ReferenceFolder referenceFolderNode = new ReferenceFolder(project);
			referenceFolderNode.AddTo(projectNode);

			WixExtensionFolderNode extensionNode = new WixExtensionFolderNode(project);
			extensionNode.AddTo(projectNode);
			
			WixLibraryFolderNode libraryNode = new WixLibraryFolderNode(project);
			libraryNode.AddTo(projectNode);

			
			return projectNode;
		}
	}
}
