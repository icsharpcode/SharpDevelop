// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class RenameEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			
			ProjectBrowserPad.Instance.ProjectBrowserControl.Select();
			ProjectBrowserPad.Instance.ProjectBrowserControl.Focus();
			ProjectBrowserPad.Instance.StartLabelEdit(node);
		}
	}
	
	public class OpenFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			
			node.ActivateItem();
		}
	}
	
	public class ExcludeFileFromProject : AbstractMenuCommand
	{
		void ExcludeFileNode(FileNode fileNode)
		{
			ProjectService.RemoveProjectItem(fileNode.Project, fileNode.ProjectItem);
			fileNode.ProjectItem = null;
			fileNode.FileNodeStatus = FileNodeStatus.None;
			if (fileNode.Parent is ExtTreeNode) {
				((ExtTreeNode)fileNode.Parent).UpdateVisibility();
			}
		}
		
		void ExcludeDirectoryNode(DirectoryNode directoryNode)
		{
			if (directoryNode.ProjectItem != null) {
				ProjectService.RemoveProjectItem(directoryNode.Project, directoryNode.ProjectItem);
				directoryNode.ProjectItem = null;
			}
			directoryNode.FileNodeStatus = FileNodeStatus.None;
		}
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			
			if (node is FileNode) {
				ExcludeFileNode((FileNode)node);
			} else if (node is DirectoryNode) {
				node.Expanding();
				Stack<TreeNode> nodeStack = new Stack<TreeNode>();
				nodeStack.Push(node);
				while (nodeStack.Count > 0) {
					TreeNode cur = nodeStack.Pop();
					
					if (cur is FileNode) {
						ExcludeFileNode((FileNode)cur);
					} else if (cur is DirectoryNode) {
						ExcludeDirectoryNode((DirectoryNode)cur);
					}
					
					foreach (TreeNode childNode in cur.Nodes) {
						if (childNode is ExtTreeNode) {
							((ExtTreeNode)childNode).Expanding();
						}
						nodeStack.Push(childNode);
					}
				}
			}
			
			ProjectService.SaveSolution();
			((AbstractProjectBrowserTreeNode)node.Parent).Refresh();
		}
	}
	
	public class IncludeFileInProject : AbstractMenuCommand
	{
		public static void IncludeFileNode(FileNode fileNode)
		{
			if (fileNode.Parent is FileNode) {
				if (((FileNode)fileNode.Parent).FileNodeStatus != FileNodeStatus.InProject) {
					IncludeFileNode((FileNode)fileNode.Parent);
				}
			}
			
			if (fileNode.Parent is DirectoryNode && !(fileNode.Parent is ProjectNode)) {
				if (((DirectoryNode)fileNode.Parent).FileNodeStatus != FileNodeStatus.InProject) {
					IncludeDirectoryNode((DirectoryNode)fileNode.Parent, false);
				}
			}
			
			ItemType type;
			if (fileNode.Project.CanCompile(fileNode.FileName)) {
				type = ItemType.Compile;
			} else {
				switch (Path.GetExtension(fileNode.FileName).ToLower()) {
					case ".resx":
					case ".resources":
						type = ItemType.EmbeddedResource;
						break;
					default:
						type = ItemType.Content;
						break;
				}
			}
			
			FileProjectItem newItem = new FileProjectItem(fileNode.Project, type);
			newItem.Include = FileUtility.GetRelativePath(fileNode.Project.Directory, fileNode.FileName);
			ProjectService.AddProjectItem(fileNode.Project, newItem);
			
			fileNode.ProjectItem    = newItem;
			fileNode.FileNodeStatus = FileNodeStatus.InProject;
			
			if (fileNode.Parent is ExtTreeNode) {
				((ExtTreeNode)fileNode.Parent).UpdateVisibility();
			}
		}
		
		public static void IncludeDirectoryNode(DirectoryNode directoryNode, bool includeSubNodes)
		{
			if (directoryNode.Parent is DirectoryNode && !(directoryNode.Parent is ProjectNode)) {
				if (((DirectoryNode)directoryNode.Parent).FileNodeStatus != FileNodeStatus.InProject) {
					IncludeDirectoryNode((DirectoryNode)directoryNode.Parent, false);
				}
			}
			if (directoryNode.Nodes.Count == 0) {
				FileProjectItem newItem = new FileProjectItem(directoryNode.Project, ItemType.Folder);
				newItem.Include = FileUtility.GetRelativePath(directoryNode.Project.Directory, directoryNode.Directory);
				ProjectService.AddProjectItem(directoryNode.Project, newItem);
				directoryNode.ProjectItem = newItem;
			}
			directoryNode.FileNodeStatus = FileNodeStatus.InProject;
			
			if (includeSubNodes) {
				foreach (TreeNode childNode in directoryNode.Nodes) {
					if (childNode is ExtTreeNode) {
						((ExtTreeNode)childNode).Expanding();
					}
					if (childNode is FileNode) {
						IncludeFileNode((FileNode)childNode);
					} else if (childNode is DirectoryNode) {
						IncludeDirectoryNode((DirectoryNode)childNode, includeSubNodes);
					}
				}
			}
		}
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return;
			}
			node.Expanding();
			
			if (node is FileNode) {
				IncludeFileNode((FileNode)node);
			} else if (node is DirectoryNode) {
				IncludeDirectoryNode((DirectoryNode)node, true);
			}
			ProjectService.SaveSolution();
			((AbstractProjectBrowserTreeNode)node.Parent).Refresh();
		}
	}
}
