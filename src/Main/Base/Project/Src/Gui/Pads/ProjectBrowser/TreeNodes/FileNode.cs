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
				projectItem = value;
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
		void SetIcon()
		{
			switch (fileNodeStatus) {
				case FileNodeStatus.None:
					SetIcon("ProjectBrowser.GhostFile");
					break;
				case FileNodeStatus.InProject:
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
		
		protected override void Initialize()
		{
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName == null) {
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
		
		public override bool EnablePaste {
			get {
				return ((ExtTreeNode)Parent).EnablePaste;
			}
		}
		public override void Delete()
		{
			if (FileNodeStatus == FileNodeStatus.Missing) {
				FileService.RemoveFile(FileName, false);
				ProjectService.SaveSolution();
			} else {
				if (MessageService.AskQuestion("Delete '" + Text + "' permanently ?")) {
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
			Clipboard.SetDataObject(new DataObject(typeof(FileNode).ToString(), new FileOperationClipboardObject(FileName, false)), true);
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			Clipboard.SetDataObject(new DataObject(typeof(FileNode).ToString(), new FileOperationClipboardObject(FileName, true)), true);
		}
		
		public override void Paste()
		{
			((ExtTreeNode)Parent).Paste();
		}
		#endregion
	}
}
