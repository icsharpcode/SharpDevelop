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
using ICSharpCode.Core.WinForms;
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
		
		public virtual string FileName {
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
			canLabelEdit = true;
		}
		
		public FileNode(string fileName) : this (fileName, FileNodeStatus.None)
		{
			sortOrder = 5;
			canLabelEdit = true;
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
			string oldFileName = FileName;
			if (oldFileName != null) {
				string newFileName = Path.Combine(Path.GetDirectoryName(oldFileName), newName);
				if (FileService.RenameFile(oldFileName, newFileName, false)) {
					Text = newName;
					this.fileName = newFileName;
					
					string oldPrefix = Path.GetFileNameWithoutExtension(oldFileName) + ".";
					string newPrefix = Path.GetFileNameWithoutExtension(newFileName) + ".";
					foreach (TreeNode node in Nodes) {
						FileNode fileNode = node as FileNode;
						if (fileNode != null) {
							FileProjectItem fileItem = fileNode.ProjectItem as FileProjectItem;
							if (fileItem != null && string.Equals(fileItem.DependentUpon, Path.GetFileName(oldFileName), StringComparison.OrdinalIgnoreCase)) {
								fileItem.DependentUpon = newName;
							}
							if (fileNode.Text.StartsWith(oldPrefix)) {
								fileNode.AfterLabelEdit(newPrefix + fileNode.Text.Substring(oldPrefix.Length));
							}
						} else {
							LoggingService.Warn("FileNode.AfterLabelEdit. Child is not a FileNode.");
						}
					}
					
					Project.Save();
				}
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
			if (Nodes.Count > 0) {
				if (MessageService.AskQuestion(GetQuestionText("${res:ProjectComponent.ContextMenu.DeleteWithDependentFiles.Question}"))) {
					DeleteChildNodes();
					FileService.RemoveFile(FileName, false);
					Project.Save();
				}
			} else if (!File.Exists(FileName)) {
				// exclude this node, then remove it
				Commands.ExcludeFileFromProject.ExcludeFileNode(this);
				this.Remove();
				Project.Save();
			} else if (MessageService.AskQuestion(GetQuestionText("${res:ProjectComponent.ContextMenu.Delete.Question}"))) {
				FileService.RemoveFile(FileName, false);
				if (IsLink) {
					// we need to manually remove the link
					Commands.ExcludeFileFromProject.ExcludeFileNode(this);
				}
				Project.Save();
			}
		}
		
		public override bool EnableCopy {
			get {
				if (base.IsEditing) {
					return false;
				}
				return true;
			}
		}
		
		public override void Copy()
		{
			ClipboardWrapper.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, false));
		}
		
		public override bool EnableCut {
			get {
				if (IsEditing) {
					return false;
				}
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
				if (IsEditing) {
					return false;
				}
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
		/// </summary>
		void DeleteChildNodes()
		{
			if (Nodes.Count == 0) return;
			
			foreach (TreeNode node in Nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null) {
					fileNode.DeleteChildNodes(); // delete recursively
					FileService.RemoveFile(fileNode.FileName, false);
				} else {
					LoggingService.Warn("FileNode.DeleteChildren. Child is not a FileNode.");
				}
			}
		}
	}
}
