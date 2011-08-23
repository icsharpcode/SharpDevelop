// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			var viewContent = FileService.OpenFile(FileName);
			if (fileNodeStatus == FileNodeStatus.Missing && viewContent != null) {
				fileNodeStatus = FileNodeStatus.InProject;
				SetIcon();
			}
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
			if (!FileService.CheckDirectoryEntryName(newName)) {
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
			if (dataObject.GetDataPresent(typeof(FileNode))) {
				// Dragging a file onto another creates a dependency.
				// If we are in the same directory, allow moving only.
				if (this.Project.ReadOnly)
					return DragDropEffects.None;
				FileNode other = (FileNode)dataObject.GetData(typeof(FileNode));
				if (other == this || !(other.ProjectItem is FileProjectItem) || !(this.ProjectItem is FileProjectItem))
					return DragDropEffects.None;
				if (FileUtility.IsEqualFileName(Path.GetDirectoryName(this.FileName), Path.GetDirectoryName(other.FileName))) {
					return DragDropEffects.Move;
				} else {
					return proposedEffect;
				}
			}
			return ((ExtTreeNode)Parent).GetDragDropEffect(dataObject, proposedEffect);
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			if (dataObject.GetDataPresent(typeof(FileNode))) {
				
				// Dragging a file onto another creates a dependency.
				
				FileNode other = (FileNode)dataObject.GetData(typeof(FileNode));
				LoggingService.Debug("ProjectBrowser: Dragging file '" + other.FileName + "' onto file '" + this.FileName + "'");
				
				// Copy/move the file to the correct directory
				// if the target is in a different directory than the source.
				if (!FileUtility.IsEqualFileName(Path.GetDirectoryName(this.FileName), Path.GetDirectoryName(other.FileName))) {
					LoggingService.Debug("-> Source file is in different directory, performing " + effect.ToString());
					ExtTreeNode p = this;
					DirectoryNode parentDirectory;
					do {
						p = (ExtTreeNode)p.Parent;
						parentDirectory = p as DirectoryNode;
					} while (parentDirectory == null && p != null);
					if (parentDirectory == null) {
						throw new InvalidOperationException("File '" + this.FileName + "' does not have a parent directory.");
					}
					LoggingService.Debug("-> Copying/Moving source file to parent directory of target: " + parentDirectory.Directory);
					string otherFileName = Path.GetFileName(other.FileName);
					parentDirectory.CopyFileHere(other, effect == DragDropEffects.Move);
					// Find the copied or moved file node again
					other = parentDirectory.AllNodes.OfType<FileNode>().SingleOrDefault(n => FileUtility.IsEqualFileName(Path.GetFileName(n.FileName), otherFileName));
				}
				
				if (other != null) {
					other.Remove();
					((FileProjectItem)other.ProjectItem).DependentUpon = Path.GetFileName(this.FileName);
					other.FileNodeStatus = FileNodeStatus.BehindFile;
					other.InsertSorted(this);
					LoggingService.Debug("-> Created new dependency, saving solution");
					ProjectService.SaveSolution();
				} else {
					LoggingService.Debug("-> Could not find the copied or moved file node in the new parent directory.");
				}
				
				return;
				
			}
			
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
			foreach (FileNode fileNode in Nodes.OfType<FileNode>().ToList()) {
				fileNode.DeleteChildNodes(); // delete recursively
				FileService.RemoveFile(fileNode.FileName, false);
			}
		}

		public override AbstractProjectBrowserTreeNode GetNodeByRelativePath(string relativePath)
		{
			if (relativePath == Text)
				return this;

			foreach (AbstractProjectBrowserTreeNode node in Nodes)
			{
				AbstractProjectBrowserTreeNode returnedNode = node.GetNodeByRelativePath(relativePath);
				if (returnedNode != null)
					return returnedNode;
			}

			return this;

		}
	}
}
