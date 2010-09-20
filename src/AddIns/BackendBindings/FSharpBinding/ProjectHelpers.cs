// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpDevelop.Project;

namespace FSharpBinding
{
	public static class ProjectHelpers
	{
		public static IEnumerable<FileNode> getFileNodes(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null && fileNode.ProjectItem != null) {
					yield return fileNode;
				}
			}
		}
		
		public static void reorderItems(TreeNodeCollection nodes, IProject project)
		{
			//ProjectService.MarkProjectDirty(project)
			project.Save();
			XmlDocument doc = new XmlDocument();
			doc.Load(project.FileName);
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003");
			var d = new Dictionary<FileNode, XmlNode>();
			foreach (FileNode node in getFileNodes(nodes)) {
				var docNode = doc.SelectSingleNode("//proj:Compile[@Include=\"" + Path.GetFileName(node.FileName) + "\"]", nsmgr);
				if (docNode != null) {
					d[node] = docNode;
					docNode.ParentNode.RemoveChild(docNode);
				}
			}
			var itemNode = doc.SelectSingleNode("//proj:ItemGroup", nsmgr);
			foreach (FileNode node in getFileNodes(nodes)) {
				XmlNode xmlElem;
				if (d.TryGetValue(node, out xmlElem))
					itemNode.AppendChild(xmlElem);
			}
			doc.Save(project.FileName);
		}
	}
	
	public class MoveUpFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			FileNode node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node != null) {
				TreeNode parent = node.Parent;
				int nodeIndex = parent.Nodes.IndexOf(node);
				if (nodeIndex > 1) {
					parent.Nodes.Remove(node);
					parent.Nodes.Insert(nodeIndex - 1, node);
				}
				ProjectHelpers.reorderItems(parent.Nodes, node.Project);
			}
		}
	}
	
	public class MoveDownFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			FileNode node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node != null) {
				TreeNode parent = node.Parent;
				int nodeIndex = parent.Nodes.IndexOf(node);
				if (nodeIndex < parent.Nodes.Count) {
					parent.Nodes.Remove(node);
					parent.Nodes.Insert(nodeIndex + 1, node);
				}
				ProjectHelpers.reorderItems(parent.Nodes, node.Project);
			}
		}
	}
}
