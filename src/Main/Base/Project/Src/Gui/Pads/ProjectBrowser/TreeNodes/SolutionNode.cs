// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionNode : AbstractProjectBrowserTreeNode, ISolutionFolderNode
	{
		ISolution solution;
		
		ISolutionFolder ISolutionFolderNode.Folder {
			get {
				return solution;
			}
		}
		
		public override ISolution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionNode(ISolution solution)
		{
			sortOrder = -1;
			this.solution = solution;
			UpdateText();;
			autoClearNodes = false;
			canLabelEdit = true;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionNode";
			
			SetIcon("ProjectBrowser.Solution");
			Tag = solution;
		}
		
		public override void BeforeLabelEdit()
		{
			Text = solution.Name;
		}
		
		public override void AfterLabelEdit(string newName)
		{
			try {
				if (solution.Name == newName)
					return;
				if (!FileService.CheckFileName(newName))
					return;
				solution.Name = newName;
			} finally {
				UpdateText();
			}
		}
		
		void UpdateText()
		{
			Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.SolutionNodeText") + " " + solution.Name;
			if (solution.IsReadOnly) {
				Text += StringParser.Parse(" (${res:Global.ReadOnly})");
			}
		}
		
		public void AddItem(string fileName)
		{
			string folderName = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.SolutionItemsNodeText");
			SolutionFolderNode node = null;
			foreach (TreeNode n in Nodes) {
				node = n as SolutionFolderNode;
				if (node != null && node.Folder.Name == folderName) {
					break;
				}
				node = null;
			}
			if (node == null) {
				ISolutionFolder newSolutionFolder = solution.CreateFolder(folderName);
				solution.Save();
				
				node = new SolutionFolderNode(newSolutionFolder);
				node.InsertSorted(this);
			}
			node.AddItem(fileName);
		}
		
		#region Drag & Drop
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				
				if (folderNode.Folder.ParentFolder != solution) {
					return DragDropEffects.All;
				}
			}
			
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				
				if (projectNode.Parent != this) {
					return DragDropEffects.Move;
				}
			}
			return DragDropEffects.None;
			
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			AbstractProjectBrowserTreeNode parentNode = null;
			
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				parentNode = folderNode.Parent as AbstractProjectBrowserTreeNode;
				
				folderNode.Remove();
				folderNode.InsertSorted(this);
				
				SolutionFolderNode.MoveItem(folderNode.Folder, this.solution);
			}
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				parentNode = projectNode.Parent as AbstractProjectBrowserTreeNode;
				
				projectNode.Remove();
				projectNode.InsertSorted(this);
				projectNode.EnsureVisible();
				SolutionFolderNode.MoveItem(projectNode.Project, this.solution);
			}
			
			if (parentNode != null) {
				parentNode.Refresh();
			}
			
			
			solution.Save();
		}
		#endregion
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		#region Cut&Paste
		public override bool EnablePaste {
			get {
				return SolutionFolderNode.DoEnablePaste(this);
			}
		}
		
		public override void Paste()
		{
			SolutionFolderNode.DoPaste(this);
		}
		#endregion
	}
}
