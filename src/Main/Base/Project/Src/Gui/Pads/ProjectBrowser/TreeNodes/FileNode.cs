// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class FileNode : AbstractProjectBrowserTreeNode, IOwnerState
	{
		string         fileName       = String.Empty;
		FileNodeStatus fileNodeStatus = FileNodeStatus.None;
		ProjectItem    projectItem    = null;
		
		public override bool Visible {
			get {
				return ShowAll || fileNodeStatus != FileNodeStatus.None;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
				Text = Path.GetFileName(fileName);
			}
		}
		
		public ProjectItem ProjectItem {
			get {
				return projectItem;
			}
			set {
				if (projectItem != value) {
					projectItem = value;
					Tag = projectItem;
					SetIcon();
				}
			}
		}
		
		public System.Enum InternalState {
			get {
				return fileNodeStatus;
			}
		}
		
		public FileNodeStatus FileNodeStatus {
			get {
				return fileNodeStatus;
			}
			set {
				if (fileNodeStatus != value) {
					fileNodeStatus = value;
					SetIcon();
				}
			}
		}
		public bool IsLink {
			get {
				return projectItem is FileProjectItem && (projectItem as FileProjectItem).IsLink;
			}
		}
		void SetIcon()
		{
			switch (fileNodeStatus) {
				case FileNodeStatus.None:
					SetIcon("ProjectBrowser.GhostFile");
					break;
				case FileNodeStatus.InProject:
					if (IsLink)
						SetIcon("ProjectBrowser.CodeBehind");
					else
						SetIcon(IconService.GetImageForFile(FileName));
					break;
				case FileNodeStatus.Missing:
					SetIcon("ProjectBrowser.MissingFile");
					break;
				case FileNodeStatus.BehindFile:
					SetIcon("ProjectBrowser.CodeBehind");
					break;
			}
		}
		
		public virtual string RelativePath {
			get {
				if (Parent is DirectoryNode) {
					return Path.Combine(((DirectoryNode)Parent).RelativePath, Text);
				}
				return Text;
			}
		}
		
		public FileNode(string fileName, FileNodeStatus fileNodeStatus)
		{
			sortOrder = 5;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/FileNode";
			ToolbarAddinTreePath     = "/SharpDevelop/Pads/ProjectBrowser/ToolBar/File";
			this.fileNodeStatus = fileNodeStatus;
			this.FileName = fileName;
			
			autoClearNodes = false;
			SetIcon();
		}
		
		public FileNode(string fileName) : this (fileName, FileNodeStatus.None)
		{
			sortOrder = 5;
		}
		
		public override void ActivateItem()
		{
			FileService.OpenFile(FileName);
		}
		
//		protected override void Initialize()
//		{
//			base.Initialize();
//		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName == null) {
				return;
			}
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			Text = newName;
			if (FileName != null) {
				string newFileName = Path.Combine(Path.GetDirectoryName(FileName), newName);
				FileService.RenameFile(FileName, newFileName, false);
				this.fileName = newFileName;
				ProjectService.SaveSolution();
			}
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		#region Drag & Drop
		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
		}
		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			return ((ExtTreeNode)Parent).GetDragDropEffect(dataObject, proposedEffect);
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			((ExtTreeNode)Parent).DoDragDrop(dataObject, effect);
		}
		#endregion
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			if (FileNodeStatus == FileNodeStatus.Missing) {
				FileService.RemoveFile(FileName, false);
				ProjectService.SaveSolution();
			} else {
				if (Nodes.Count > 0) {
					if (MessageService.AskQuestion("Delete '" + Text + "' and its dependent files permanently?")) {
						DeleteChildNodes();
						FileService.RemoveFile(FileName, false);
						ProjectService.SaveSolution();
					}
				}
				else if (MessageService.AskQuestion("Delete '" + Text + "' permanently ?")) {
					FileService.RemoveFile(FileName, false);
					ProjectService.SaveSolution();
				}
			}
		}
		
		public override bool EnableCopy {
			get {
				return true;
			}
		}
		
		public override void Copy()
		{
			ClipboardWrapper.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, false));
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			ClipboardWrapper.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, true));
		}
		
		
		public override bool EnablePaste {
			get {
				return ((ExtTreeNode)Parent).EnablePaste;
			}
		}
		public override void Paste()
		{
			((ExtTreeNode)Parent).Paste();
		}
		#endregion

		/// <summary>
		/// Deletes all dependent child nodes and their associated files.
		/// Does not delete recursively - not required as yet.
		/// </summary>
		void DeleteChildNodes()
		{
			if (Nodes.Count == 0) return;
			
			foreach (TreeNode node in Nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null) {
					FileService.RemoveFile(fileNode.FileName, false);
				} else {
					LoggingService.Warn("FileNode.DeleteChildren. Child is not a FileNode.");
				}
			}
		}
	}
}
