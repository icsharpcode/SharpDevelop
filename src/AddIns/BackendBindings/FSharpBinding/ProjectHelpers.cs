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
			
			SaveProjectXml(doc, project as FSharpProject);
		}
		
		static void SaveProjectXml(XmlDocument doc, FSharpProject project)
		{
			project.DisableWatcher();
			try {
				doc.Save(project.FileName);
			} finally {
				project.EnableWatcher();
			}
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
