// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
