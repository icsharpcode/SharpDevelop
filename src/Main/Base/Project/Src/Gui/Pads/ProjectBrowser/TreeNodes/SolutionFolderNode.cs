// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public interface ISolutionFolderNode
	{
		Solution Solution {
			get;
		}
		
		ISolutionFolder Folder {
			get;
		}
		
		ISolutionFolderContainer Container {
			get;
		}
		
		void AddItem(string fileName);
	}
	
	public class SolutionFolderNode : CustomFolderNode, ISolutionFolderNode
	{
		Solution       solution;
		SolutionFolder folder;
		
		public override Solution Solution {
			get {
				Debug.Assert(solution != null);
				return solution;
			}
		}
		
		public ISolutionFolder Folder {
			get {
				Debug.Assert(folder != null);
				return folder;
			}
		}
		
		public ISolutionFolderContainer Container {
			get {
				return folder;
			}
		}
		
		public SolutionFolderNode(Solution solution, SolutionFolder folder)
		{
			sortOrder = 0;
			canLabelEdit = true;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionFolderNode";
			this.solution  = solution;
			this.folder    = folder;
			this.Tag       = folder;
			Text           = folder.Name;
			autoClearNodes = false;
			
			OpenedImage = "ProjectBrowser.SolutionFolder.Open";
			ClosedImage = "ProjectBrowser.SolutionFolder.Closed";
			Initialize();
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			Text = folder.Location = folder.Name = newName;
			solution.Save();
		}
		
		public void AddItem(string fileName)
		{
			string relativeFileName = FileUtility.GetRelativePath(solution.Directory, fileName);
			SolutionItem newItem = new SolutionItem(relativeFileName, relativeFileName);
			folder.SolutionItems.Items.Add(newItem);
			new SolutionItemNode(solution, newItem).InsertSorted(this);
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			
			foreach (object treeObject in folder.Folders) {
				if (treeObject is IProject) {
					NodeBuilders.AddProjectNode(this, (IProject)treeObject);
				} else if (treeObject is SolutionFolder) {
					SolutionFolderNode folderNode = new SolutionFolderNode(solution, (SolutionFolder)treeObject);
					folderNode.InsertSorted(this);
				} else {
					MessageService.ShowWarning("SolutionFolderNode.Initialize(): unknown tree object : " + treeObject);
				}
			}
			
			// add solution items (=files) from project sections.
			foreach (SolutionItem item in folder.SolutionItems.Items) {
				new SolutionItemNode(Solution, item).InsertSorted(this);
			}
			base.Initialize();
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			ProjectService.RemoveSolutionFolder(folder.IdGuid);
			solution.Save();
		}
		
		public override bool EnableCopy {
			get {
				return false;
			}
		}
		public override void Copy()
		{
			throw new System.NotSupportedException();
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			ClipboardWrapper.SetDataObject(new DataObject(typeof(ISolutionFolder).ToString(), folder.IdGuid));
		}
		
		public static bool DoEnablePaste(ISolutionFolderNode container)
		{
			return DoEnablePaste(container, ClipboardWrapper.GetDataObject());
		}
		
		static bool DoEnablePaste(ISolutionFolderNode container, IDataObject dataObject)
		{
			if (dataObject == null) {
				return false;
			}
			if (dataObject.GetDataPresent(typeof(ISolutionFolder).ToString())) {
				string guid = dataObject.GetData(typeof(ISolutionFolder).ToString()).ToString();
				ISolutionFolder solutionFolder = container.Solution.GetSolutionFolder(guid);
				if (solutionFolder == null || solutionFolder == container)
					return false;
				if (solutionFolder is ISolutionFolderContainer) {
					return solutionFolder.Parent != container
						&& !((ISolutionFolderContainer)solutionFolder).IsAncestorOf(container.Folder);
				} else {
					return solutionFolder.Parent != container;
				}
			}
			return false;
		}
		
		public static void DoPaste(ISolutionFolderNode folderNode)
		{
			IDataObject dataObject = ClipboardWrapper.GetDataObject();
			if (!DoEnablePaste(folderNode, dataObject)) {
				LoggingService.Warn("SolutionFolderNode.DoPaste: Pasting was not enabled.");
				return;
			}
			
			ExtTreeNode folderTreeNode = (ExtTreeNode)folderNode;
			
			if (dataObject.GetDataPresent(typeof(ISolutionFolder).ToString())) {
				string guid = dataObject.GetData(typeof(ISolutionFolder).ToString()).ToString();
				ISolutionFolder solutionFolder = folderNode.Solution.GetSolutionFolder(guid);
				if (solutionFolder != null) {
					folderNode.Container.AddFolder(solutionFolder);
					ExtTreeView treeView = (ExtTreeView)folderTreeNode.TreeView;
					foreach (ExtTreeNode node in treeView.CutNodes) {
						ExtTreeNode oldParent = node.Parent as ExtTreeNode;
						node.Remove();
						
						node.InsertSorted(folderTreeNode);
						if (oldParent != null) {
							oldParent.Refresh();
						}
					}
					ProjectService.SaveSolution();
				}
			}
			folderTreeNode.Expand();
		}
		
		public override bool EnablePaste {
			get {
				return DoEnablePaste(this);
			}
		}
		
		public override void Paste()
		{
			DoPaste(this);
		}
		#endregion
		
		#region Drag & Drop
		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
		}
		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				
				if (folderNode.Folder.Parent != this.folder && !folderNode.Container.IsAncestorOf(Folder)) {
					return DragDropEffects.Move;
				}
			}
			
			if (dataObject.GetDataPresent(typeof(SolutionItemNode))) {
				SolutionItemNode solutionItemNode = (SolutionItemNode)dataObject.GetData(typeof(SolutionItemNode));
				
				if (solutionItemNode.Parent != this) {
					return DragDropEffects.Move;
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
			if (!isInitialized) {
				Initialize();
				isInitialized = true;
			}
			
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				AbstractProjectBrowserTreeNode parentNode = folderNode.Parent as AbstractProjectBrowserTreeNode;
				
				folderNode.Remove();
				folderNode.InsertSorted(this);
				folderNode.EnsureVisible();
				this.folder.AddFolder(folderNode.Folder);
				if (parentNode != null) {
					parentNode.Refresh();
				}
			}
			
			if (dataObject.GetDataPresent(typeof(SolutionItemNode))) {
				SolutionItemNode solutionItemNode = (SolutionItemNode)dataObject.GetData(typeof(SolutionItemNode));
				
				ISolutionFolderNode folderNode = (ISolutionFolderNode)solutionItemNode.Parent;
				folderNode.Container.SolutionItems.Items.Remove(solutionItemNode.SolutionItem);
				Container.SolutionItems.Items.Add(solutionItemNode.SolutionItem);
				
				solutionItemNode.Remove();
				solutionItemNode.InsertSorted(this);
				solutionItemNode.EnsureVisible();
				if (solutionItemNode.Parent != null) {
					((ExtTreeNode)solutionItemNode.Parent).Refresh();
				}
			}
			
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				
				projectNode.Remove();
				projectNode.InsertSorted(this);
				projectNode.EnsureVisible();
				this.folder.AddFolder(projectNode.Project);
				
				if (projectNode.Parent != null) {
					((ExtTreeNode)projectNode.Parent).Refresh();
				}
			}
			
			solution.Save();
			
			
		}
		#endregion
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
