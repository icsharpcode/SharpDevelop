// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	[Serializable]
	public class FileOperationClipboardObject : System.MarshalByRefObject
	{
		string fileName;
		bool   performMove;
		public string FileName {
			get {
				return fileName;
			}
		}
		public bool PerformMove {
			get {
				return performMove;
			}
		}
		public FileOperationClipboardObject(string fileName, bool performMove)
		{
			this.fileName = fileName;
			this.performMove = performMove;
		}
	}
	
	public enum SpecialFolder {
		None,
		AppDesigner,
		WebReference,
		WebReferenceFolder
	}
	
	public class DirectoryNode : AbstractProjectBrowserTreeNode, IOwnerState
	{
		string closedImage = null;
		string openedImage = null;
		FileNodeStatus fileNodeStatus = FileNodeStatus.None;
		SpecialFolder  specialFolder  = SpecialFolder.None;
		ProjectItem projectItem = null;
		
		public override bool Visible {
			get {
				return ShowAll || fileNodeStatus != FileNodeStatus.None;
			}
		}
		
		public SpecialFolder SpecialFolder {
			get {
				if (Parent is DirectoryNode && ((DirectoryNode)Parent).SpecialFolder == SpecialFolder.WebReferenceFolder) {
					return SpecialFolder.WebReference;
				}
				return specialFolder;
			}
			set {
				if (specialFolder != value) {
					specialFolder = value;
					SetIcon();
				}
			}
		}
		
		public string ClosedImage {
			get {
				return closedImage;
			}
			set {
				closedImage = value;
				if (!IsExpanded) {
					SetIcon(closedImage);
				}
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
		
		public string OpenedImage {
			get {
				return openedImage;
			}
			set {
				openedImage = value;
				if (IsExpanded) {
					SetIcon(openedImage);
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
		public override void Refresh()
		{
			base.Refresh();
			if (Nodes.Count == 0) {
				SetIcon(ClosedImage);
			} else if (IsExpanded) {
				SetIcon(openedImage);
			}
		}
		
		
		void SetIcon()
		{
			switch (fileNodeStatus) {
				case FileNodeStatus.None:
					OpenedImage = "ProjectBrowser.GhostFolder.Open";
					ClosedImage = "ProjectBrowser.GhostFolder.Closed";
					break;
				default:
					switch (SpecialFolder) {
						case SpecialFolder.None:
							OpenedImage = "ProjectBrowser.Folder.Open";
							ClosedImage = "ProjectBrowser.Folder.Closed";
							break;
						case SpecialFolder.AppDesigner:
							OpenedImage = "ProjectBrowser.PropertyFolder.Open";
							ClosedImage = "ProjectBrowser.PropertyFolder.Closed";
							break;
						case SpecialFolder.WebReferenceFolder:
							OpenedImage = "ProjectBrowser.WebReferenceFolder.Open";
							ClosedImage = "ProjectBrowser.WebReferenceFolder.Closed";
							break;
						case SpecialFolder.WebReference:
							OpenedImage = "ProjectBrowser.WebReference";
							ClosedImage = "ProjectBrowser.WebReference";
							break;
					}
					break;
			}
		}
		
		string directory = String.Empty;
		public virtual string Directory {
			get {
				return directory;
			}
			set {
				directory = value;
				Text = Path.GetFileName(directory);
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
		
		protected DirectoryNode()
		{
			sortOrder = 1;
			SetIcon();
		}
		
		public DirectoryNode(string directory) : this(directory, FileNodeStatus.None)
		{
			sortOrder = 1;
		}
		CustomNode removeMe = null;
		public DirectoryNode(string directory, FileNodeStatus fileNodeStatus)
		{
			sortOrder = 1;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/FolderNode";
			this.Directory = directory;
			this.fileNodeStatus = fileNodeStatus;
			if (fileNodeStatus != FileNodeStatus.Missing) {
				if (System.IO.Directory.GetDirectories(Directory).Length > 0 || System.IO.Directory.GetFiles(Directory).Length > 0) {
					removeMe = new CustomNode();
					removeMe.AddTo(this);
				}
			}
			SetIcon();
		}
		
		protected override void Initialize()
		{
			if (removeMe != null) {
				Nodes.Remove(removeMe);
				removeMe = null;
			}
			
			LoggingService.Info("Initialize DirectoryNode " + Directory);
			
			Dictionary<string, FileNode> fileNodeDictionary
				= new Dictionary<string, FileNode>(StringComparer.InvariantCultureIgnoreCase);
			Dictionary<FileNode, string> dependendFileDictionary = new Dictionary<FileNode, string>();
			Dictionary<string, DirectoryNode> directoryNodeList = new Dictionary<string, DirectoryNode>(StringComparer.InvariantCultureIgnoreCase);
			
			// Add files found in file system
			
			if (System.IO.Directory.Exists(Directory)) {
				foreach (string subDirectory in System.IO.Directory.GetDirectories(Directory)) {
					if (Path.GetFileName(subDirectory) != ".svn") {
						DirectoryNode newDirectoryNode = new DirectoryNode(subDirectory);
						if (subDirectory == Path.Combine(Project.Directory, Project.AppDesignerFolder)) {
							newDirectoryNode.SpecialFolder = SpecialFolder.AppDesigner;
						}
						newDirectoryNode.AddTo(this);
						directoryNodeList[Path.GetFileName(subDirectory)] = newDirectoryNode;
					}
				}
				
				foreach (string file in System.IO.Directory.GetFiles(Directory)) {
					FileNode fileNode = new FileNode(file);
					fileNodeDictionary[Path.GetFileName(file)] = fileNode;
					fileNode.AddTo(this);
				}
			}
			if (Nodes.Count == 0) {
				SetClosedImage();
			}
			
			string relativeDirectoryPath = this.RelativePath;
			if (relativeDirectoryPath.Length > 0)
				relativeDirectoryPath = relativeDirectoryPath.Replace('\\', '/') + '/';
			
			// Add project items
			
			foreach (ProjectItem item in Project.Items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem == null)
					continue;
				string virtualName = fileItem.VirtualName.Replace('\\', '/');
				string fileName = Path.GetFileName(virtualName);
				if (!string.Equals(virtualName, relativeDirectoryPath + fileName, StringComparison.InvariantCultureIgnoreCase)) {
					AddParentFolder(virtualName, relativeDirectoryPath, directoryNodeList);
					continue;
				}
				
				if (item.ItemType == ItemType.Folder || item.ItemType == ItemType.WebReferences) {
					DirectoryNode node;
					if (directoryNodeList.TryGetValue(fileName, out node)) {
						node.FileNodeStatus = FileNodeStatus.InProject;
					} else {
						new DirectoryNode(item.FileName.Trim('\\', '/'), FileNodeStatus.Missing).AddTo(this);
					}
				} else {
					FileNode node;
					if (fileItem.IsLink) {
						node = new FileNode(fileItem.FileName, FileNodeStatus.InProject);
						node.AddTo(this);
						fileNodeDictionary[fileName] = node;
					} else {
						if (fileNodeDictionary.TryGetValue(fileName, out node)) {
							node.FileNodeStatus = FileNodeStatus.InProject;
						} else {
							node = new FileNode(fileItem.FileName, FileNodeStatus.Missing);
							node.AddTo(this);
							fileNodeDictionary[fileName] = node;
						}
					}
					
					node.ProjectItem = fileItem;
					if (fileItem != null && fileItem.DependentUpon != null && fileItem.DependentUpon.Length > 0) {
						dependendFileDictionary[node] = fileItem.DependentUpon;
					}
				}
			}
			
			// Insert 'code behind files'
			foreach (KeyValuePair<FileNode, string> pair in dependendFileDictionary) {
				string fileName = Path.GetFileName(pair.Value);
				if (!fileNodeDictionary.ContainsKey(fileName)) {
					continue;
				}
				AbstractProjectBrowserTreeNode parentNode = fileNodeDictionary[fileName];
				pair.Key.Parent.Nodes.Remove(pair.Key);
				pair.Key.AddTo(parentNode);
				if (pair.Key.FileNodeStatus != FileNodeStatus.Missing) {
					pair.Key.FileNodeStatus = FileNodeStatus.BehindFile;
				}
			}
			base.Initialize();
		}
		
		void AddParentFolder(string virtualName, string relativeDirectoryPath, Dictionary<string, DirectoryNode> directoryNodeList)
		{
			if (relativeDirectoryPath.Length == 0
			    || string.Compare(virtualName, 0, relativeDirectoryPath, 0, relativeDirectoryPath.Length, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				// virtualName is a file in this folder, so we have to add its containing folder
				// to the project
				int pos = virtualName.IndexOf('/', relativeDirectoryPath.Length + 1);
				if (pos < 0)
					return;
				string subFolderName = virtualName.Substring(relativeDirectoryPath.Length, pos - relativeDirectoryPath.Length);
				DirectoryNode node;
				if (directoryNodeList.TryGetValue(subFolderName, out node)) {
					node.FileNodeStatus = FileNodeStatus.InProject;
				} else {
					new DirectoryNode(Path.Combine(Directory, subFolderName), FileNodeStatus.Missing).AddTo(this);
				}
			}
		}
		
		void SetOpenedImage()
		{
			if (openedImage != null) {
				SetIcon(openedImage);
			}
		}
		
		void SetClosedImage()
		{
			if (closedImage != null) {
				SetIcon(closedImage);
			}
		}
		public override void Expanding()
		{
			SetOpenedImage();
			base.Expanding();
		}
		
		public override void Collapsing()
		{
			SetClosedImage();
			base.Collapsing();
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName == null) {
				return;
			}
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			string oldText = Text;
			Text = newName;
			if (Directory != null) {
				string newPath = Path.Combine(Path.GetDirectoryName(Directory), newName);
				if (System.IO.Directory.Exists(newPath)) {
					if (System.IO.Directory.GetFiles(Directory).Length == 0) {
						System.IO.Directory.Delete(Directory);
					} else if (System.IO.Directory.GetFiles(newPath).Length == 0) {
						System.IO.Directory.Delete(newPath);
						FileService.RenameFile(Directory, newPath, true);
					} else {
						MessageService.ShowError("The folder already exists and contains files!");
						Text = oldText;
						return;
					}
				} else {
					FileService.RenameFile(Directory, newPath, true);
				}
				
				this.directory = newPath;
				ProjectService.SaveSolution();
			}
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			if (FileNodeStatus == FileNodeStatus.Missing) {
				FileService.RemoveFile(Directory, true);
				ProjectService.SaveSolution();
			} else {
				if (MessageService.AskQuestion("Delete '" + Text + "' and all its contents permanently ?")) {
					FileService.RemoveFile(Directory, true);
					ProjectService.SaveSolution();
				}
			}
		}
		
		public override bool EnablePaste {
			get {
				IDataObject dataObject = ClipboardWrapper.GetDataObject();
				if (dataObject == null) {
					return false;
				}
				if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
					return true;
				}
				if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(FileNode).ToString());
					return !FileUtility.IsEqualFileName(Directory, clipboardObject.FileName) && !FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(clipboardObject.FileName)) && File.Exists(clipboardObject.FileName);
				}
				if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(DirectoryNode).ToString());
					if (FileUtility.IsBaseDirectory(clipboardObject.FileName, Directory)) {
						return false;
					}
					return !FileUtility.IsEqualFileName(Directory, clipboardObject.FileName) && !FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(clipboardObject.FileName)) && System.IO.Directory.Exists(clipboardObject.FileName);
				}
				return false;
			}
		}
		
		public override void Paste()
		{
			IDataObject dataObject = ClipboardWrapper.GetDataObject();
			
			if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
				foreach (string fileName in files) {
					if (System.IO.Directory.Exists(fileName)) {
						if (!FileUtility.IsBaseDirectory(fileName, Directory)) {
							ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyDirectory(fileName, this);
						}
					} else {
						ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyFile(fileName, this, true);
					}
				}
			} else if (dataObject.GetDataPresent(typeof(FileNode))) {
				FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(FileNode).ToString());
				
				if (File.Exists(clipboardObject.FileName)) {
					ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyFile(clipboardObject.FileName, this, true);
					if (clipboardObject.PerformMove) {
						FileService.RemoveFile(clipboardObject.FileName, false);
					}
				}
			} else if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
				FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(DirectoryNode).ToString());
				
				if (System.IO.Directory.Exists(clipboardObject.FileName)) {
					ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyDirectory(clipboardObject.FileName, this);
					if (clipboardObject.PerformMove) {
						FileService.RemoveFile(clipboardObject.FileName, true);
						dataObject.SetData(null);
					}
				}
			}
			ProjectService.SaveSolution();
		}
		
		public override bool EnableCopy {
			get {
				return true;
			}
		}
		public override void Copy()
		{
			ClipboardWrapper.SetDataObject(new DataObject(typeof(DirectoryNode).ToString(), new FileOperationClipboardObject(Directory, false)));
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			ClipboardWrapper.SetDataObject(new DataObject(typeof(DirectoryNode).ToString(), new FileOperationClipboardObject(Directory, true)));
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
			if (dataObject.GetDataPresent(typeof(FileNode))) {
				FileNode fileNode = (FileNode)dataObject.GetData(typeof(FileNode));
				
				if (!FileUtility.IsEqualFileName(Directory, fileNode.FileName) && !FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(fileNode.FileName))) {
					if (Project != fileNode.Project) {
						return DragDropEffects.Copy;
					}
					return proposedEffect;
				}
			}
			
			if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
				DirectoryNode directoryNode = (DirectoryNode)dataObject.GetData(typeof(DirectoryNode));
				if (FileUtility.IsBaseDirectory(directoryNode.Directory, Directory)) {
					return DragDropEffects.None;
				}
				if (!FileUtility.IsEqualFileName(Directory, directoryNode.Directory) && !FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(directoryNode.Directory))) {
					if (Project != directoryNode.Project) {
						return DragDropEffects.Copy;
					}
					return proposedEffect;
				}
			}
			if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				return DragDropEffects.Copy;
			}
			return DragDropEffects.None;
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			PerformInitialization();
			Expand();
			try {
				if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
					foreach (string fileName in files) {
						if (System.IO.Directory.Exists(fileName)) {
							if (!FileUtility.IsBaseDirectory(fileName, Directory)) {
								ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyDirectory(fileName, this);
							}
						} else {
							ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyFile(fileName, this, true);
						}
					}
				} else if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileNode fileNode = (FileNode)dataObject.GetData(typeof(FileNode));
					ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyFile(fileNode.FileName, this, fileNode.FileNodeStatus != FileNodeStatus.None);
					if (effect == DragDropEffects.Move) {
						FileService.RemoveFile(fileNode.FileName, false);
					}
				} else if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					DirectoryNode directoryNode = (DirectoryNode)dataObject.GetData(typeof(DirectoryNode));
					ICSharpCode.SharpDevelop.Project.Commands.AddExistingItemsToProject.CopyDirectory(directoryNode.Directory, this);
					if (effect == DragDropEffects.Move) {
						FileService.RemoveFile(directoryNode.Directory, true);
					}
				}
				
				ProjectService.SaveSolution();
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		#endregion
	}
}
