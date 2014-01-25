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

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project
{
	[Serializable]
	public class FileOperationClipboardObject
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
		
		public static System.Windows.IDataObject CreateDataObject(FileNode node, bool performMove)
		{
			return new System.Windows.DataObject(typeof(FileNode).ToString(), new FileOperationClipboardObject(node.FileName, performMove));
		}
		
		public static System.Windows.IDataObject CreateDataObject(SolutionItemNode node, bool performMove)
		{
			return new System.Windows.DataObject(typeof(SolutionItemNode).ToString(),
			                      new FileOperationClipboardObject(node.FileName, performMove));
		}
		
		public static System.Windows.IDataObject CreateDataObject(DirectoryNode node, bool performMove)
		{
			return new System.Windows.DataObject(typeof(DirectoryNode).ToString(),
			                      new FileOperationClipboardObject(node.Directory, performMove));
		}
	}
	
	//TODO: Maybe I need to add an enum member for the properties folder.
	public enum SpecialFolder {
		None,
		AppDesigner,
		ServiceReference,
		ServiceReferencesFolder,
		WebReference,
		WebReferencesFolder
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
				if (projectItem != null && projectItem.ItemType == ItemType.WebReferenceUrl) {
					Tag = projectItem;
				}
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
				case FileNodeStatus.Missing:
					OpenedImage = "ProjectBrowser.Folder.Missing";
					ClosedImage = "ProjectBrowser.Folder.Missing";
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
						case SpecialFolder.ServiceReferencesFolder:
						case SpecialFolder.WebReferencesFolder:
							OpenedImage = "ProjectBrowser.WebReferenceFolder.Open";
							ClosedImage = "ProjectBrowser.WebReferenceFolder.Closed";
							break;
						case SpecialFolder.ServiceReference:
						case SpecialFolder.WebReference:
							OpenedImage = "ProjectBrowser.WebReference";
							ClosedImage = "ProjectBrowser.WebReference";
							break;
					}
					break;
			}
		}
		
		DirectoryName directory;
		public virtual DirectoryName Directory {
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
			canLabelEdit = true;
		}
		
		public DirectoryNode(string directory)
			: this(directory, FileNodeStatus.None)
		{
			sortOrder = 1;
			canLabelEdit = true;
		}
		
		public DirectoryNode(string directory, FileNodeStatus fileNodeStatus)
			: this(directory, fileNodeStatus, null)
		{
		}
		
		CustomNode removeMe = null;
		
		public DirectoryNode(string directory, FileNodeStatus fileNodeStatus, ProjectItem projectItem)
		{
			sortOrder = 1;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/FolderNode";
			this.Directory = DirectoryName.Create(directory);
			this.fileNodeStatus = fileNodeStatus;
			this.ProjectItem = projectItem;
			
			removeMe = new CustomNode();
			removeMe.AddTo(this);
			
			SetIcon();
			canLabelEdit = true;
		}
		
		public void RecreateSubNodes()
		{
			invisibleNodes.Clear();
			if (autoClearNodes) {
				Nodes.Clear();
			} else {
				List<TreeNode> removedNodes = new List<TreeNode>();
				foreach (TreeNode node in Nodes) {
					if (node is FileNode || node is DirectoryNode)
						removedNodes.Add(node);
				}
				foreach (TreeNode node in removedNodes) {
					Nodes.Remove(node);
				}
			}
			Initialize();
			UpdateVisibility();
		}
		
		protected override void Initialize()
		{
			if (removeMe != null) {
				Nodes.Remove(removeMe);
				removeMe = null;
			}
			
			LoggingService.Info("Initialize DirectoryNode " + Directory);
			
			Dictionary<string, FileNode> fileNodeDictionary
				= new Dictionary<string, FileNode>(StringComparer.OrdinalIgnoreCase);
			Dictionary<FileNode, string> dependendFileDictionary = new Dictionary<FileNode, string>();
			Dictionary<string, DirectoryNode> directoryNodeList = new Dictionary<string, DirectoryNode>(StringComparer.OrdinalIgnoreCase);
			
			// Add files found in file system
			
			if (System.IO.Directory.Exists(Directory)) {
				foreach (string subDirectory in System.IO.Directory.GetDirectories(Directory)) {
					if (Path.GetFileName(subDirectory) != ".svn") {
						DirectoryNode newDirectoryNode = DirectoryNodeFactory.CreateDirectoryNode(this, Project, subDirectory);
						newDirectoryNode.InsertSorted(this);
						directoryNodeList[Path.GetFileName(subDirectory)] = newDirectoryNode;
					}
				}
				
				foreach (string file in System.IO.Directory.GetFiles(Directory)) {
					FileNode fileNode = new FileNode(file);
					fileNodeDictionary[Path.GetFileName(file)] = fileNode;
					fileNode.InsertSorted(this);
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
				if (item.ItemType == ItemType.WebReferenceUrl) {
					DirectoryNode node;
					if (directoryNodeList.TryGetValue(Path.GetFileName(item.FileName), out node)) {
						if (node.FileNodeStatus == FileNodeStatus.None) {
							node.FileNodeStatus = FileNodeStatus.InProject;
						}
						node.ProjectItem = item;
					}
					continue;
				}
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem == null)
					continue;
				string virtualName = fileItem.VirtualName.Replace('\\', '/');
				if (virtualName.EndsWith("/"))
					virtualName = virtualName.Substring(0, virtualName.Length - 1);
				string fileName = Path.GetFileName(virtualName);
				if (!string.Equals(virtualName, relativeDirectoryPath + fileName, StringComparison.OrdinalIgnoreCase)) {
					AddParentFolder(virtualName, relativeDirectoryPath, directoryNodeList);
					continue;
				}
				
				if (item.ItemType.IsFolder()) {
					DirectoryNode node;
					if (directoryNodeList.TryGetValue(fileName, out node)) {
						if (node.FileNodeStatus == FileNodeStatus.None) {
							node.FileNodeStatus = FileNodeStatus.InProject;
						}
						node.ProjectItem = item;
					} else {
						node = DirectoryNodeFactory.CreateDirectoryNode(item, FileNodeStatus.Missing);
						node.InsertSorted(this);
						directoryNodeList[fileName] = node;
					}
				} else {
					FileNode node;
					if (fileItem.IsLink) {
						node = new FileNode(fileItem.FileName, FileNodeStatus.InProject);
						node.InsertSorted(this);
						fileNodeDictionary[fileName] = node;
					} else {
						if (fileNodeDictionary.TryGetValue(fileName, out node)) {
							if (node.FileNodeStatus == FileNodeStatus.None) {
								node.FileNodeStatus = FileNodeStatus.InProject;
							}
						} else {
							node = new FileNode(fileItem.FileName, FileNodeStatus.Missing);
							node.InsertSorted(this);
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
				if (NodeIsParent(parentNode, pair.Key)) {
					// is pair.Key a parent of parentNode?
					// if yes, we have a parent cycle - break it by adding one node to the directory
					pair.Key.InsertSorted(this);
				} else {
					pair.Key.InsertSorted(parentNode);
					if (pair.Key.FileNodeStatus != FileNodeStatus.Missing) {
						pair.Key.FileNodeStatus = FileNodeStatus.BehindFile;
					}
				}
			}
			base.Initialize();
		}
		
		static bool NodeIsParent(TreeNode childNode, TreeNode parentNode)
		{
			do {
				if (childNode == parentNode) return true;
				childNode = childNode.Parent;
			} while (childNode != null);
			return false;
		}
		
		protected void BaseInitialize()
		{
			base.Initialize();
		}
		
		/// <summary>
		/// Create's a new FileProjectItem in this DirectoryNode.
		/// </summary>
		/// <param name="fileName">The name of the file that will be added to the project.</param>
		public FileProjectItem AddNewFile(string fileName)
		{
			//TODO: this can probably be moved to AbstractProjectBrowserTreeNode or even lower in the chain.
			this.Expanding();
			
			FileNode fileNode = new FileNode(fileName, FileNodeStatus.InProject);
			fileNode.InsertSorted(this);
			fileNode.EnsureVisible();
			return IncludeFileInProject.IncludeFileNode(fileNode);
		}
		
		void AddParentFolder(string virtualName, string relativeDirectoryPath, Dictionary<string, DirectoryNode> directoryNodeList)
		{
			if (relativeDirectoryPath.Length == 0
			    || string.Compare(virtualName, 0, relativeDirectoryPath, 0, relativeDirectoryPath.Length, StringComparison.OrdinalIgnoreCase) == 0)
			{
				// virtualName is a file in this folder, so we have to add its containing folder
				// to the project
				int pos = virtualName.IndexOf('/', relativeDirectoryPath.Length + 1);
				if (pos < 0)
					return;
				string subFolderName = virtualName.Substring(relativeDirectoryPath.Length, pos - relativeDirectoryPath.Length);
				DirectoryNode node;
				if (directoryNodeList.TryGetValue(subFolderName, out node)) {
					if (node.FileNodeStatus == FileNodeStatus.None) {
						node.FileNodeStatus = FileNodeStatus.InProject;
					}
				} else {
					node = new DirectoryNode(Path.Combine(Directory, subFolderName), FileNodeStatus.Missing);
					node.InsertSorted(this);
					directoryNodeList[subFolderName] = node;
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
			if (!FileService.CheckDirectoryEntryName(newName)) {
				return;
			}
			if (String.Compare(Text, newName, true) == 0) {
				return;
			}
			string oldText = Text;
			Text = newName;
			if (Directory != null) {
				string newPath = Path.Combine(Path.GetDirectoryName(Directory), newName);
				if (System.IO.Directory.Exists(newPath)) {
					if (System.IO.Directory.GetFileSystemEntries(newPath).Length == 0) {
						System.IO.Directory.Delete(newPath);
					} else {
						MessageService.ShowError("The folder already exists and contains files!");
						Text = oldText;
						return;
					}
				}
				if (!FileService.RenameFile(Directory, newPath, true)) {
					Text = oldText;
					return;
				}
				
				this.directory = DirectoryName.Create(newPath);
				Project.Save();
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
				Project.Save();
			} else {
				if (MessageService.AskQuestion(GetQuestionText("${res:ProjectComponent.ContextMenu.DeleteWithContents.Question}"))) {
					FileService.RemoveFile(Directory, true);
					Project.Save();
				}
			}
		}
		
		public override bool EnablePaste {
			get {
				System.Windows.IDataObject dataObject = SD.Clipboard.GetDataObject();
				if (dataObject == null) {
					return false;
				}
				if (dataObject.GetDataPresent(System.Windows.DataFormats.FileDrop)) {
					return true;
				}
				if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileOperationClipboardObject clipboardObject = dataObject.GetData(typeof(FileNode).ToString()) as FileOperationClipboardObject;
					return clipboardObject != null && File.Exists(clipboardObject.FileName);
				}
				if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					FileOperationClipboardObject clipboardObject = dataObject.GetData(typeof(DirectoryNode).ToString()) as FileOperationClipboardObject;
					if (clipboardObject == null)
						return false;
					if (FileUtility.IsBaseDirectory(clipboardObject.FileName, Directory)) {
						return false;
					}
					return System.IO.Directory.Exists(clipboardObject.FileName);
				}
				return false;
			}
		}
		
		public override void Paste()
		{
			System.Windows.IDataObject dataObject = SD.Clipboard.GetDataObject();
			if (dataObject == null)
				return;
			
			if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
				foreach (string fileName in files) {
					if (System.IO.Directory.Exists(fileName)) {
						if (!FileUtility.IsBaseDirectory(fileName, Directory)) {
							CopyDirectoryHere(fileName, false);
						}
					} else {
						CopyFileHere(fileName, false);
					}
				}
			} else if (dataObject.GetDataPresent(typeof(FileNode))) {
				FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(FileNode).ToString());
				
				if (File.Exists(clipboardObject.FileName)) {
					CopyFileHere(clipboardObject.FileName, clipboardObject.PerformMove);
					if (clipboardObject.PerformMove) {
						Clipboard.Clear();
					}
				}
			} else if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
				FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(DirectoryNode).ToString());
				
				if (System.IO.Directory.Exists(clipboardObject.FileName)) {
					CopyDirectoryHere(clipboardObject.FileName, clipboardObject.PerformMove);
					if (clipboardObject.PerformMove) {
						Clipboard.Clear();
					}
				}
			}
			ProjectService.SaveSolution();
		}
		
		public void CopyDirectoryHere(string directoryName, bool performMove)
		{
			string copiedName = Path.Combine(Directory, Path.GetFileName(directoryName));
			if (FileUtility.IsEqualFileName(directoryName, copiedName))
				return;
			if (performMove) {
				FileService.RenameFile(directoryName, copiedName, true);
				RecreateSubNodes();
				Expand();
			} else {
				AddExistingItemsToProject.CopyDirectory(directoryName, this, true);
			}
		}
		
		public void CopyDirectoryHere(DirectoryNode node, bool performMove)
		{
			CopyDirectoryHere(node.Directory, performMove);
		}
		
		/// <summary>
		/// Copies or moves a file to this directory, discarding its DependentUpon value.
		/// </summary>
		/// <param name="fileName">The name of the file to copy or move.</param>
		/// <param name="performMove">true to move the file, false to copy it.</param>
		public void CopyFileHere(string fileName, bool performMove)
		{
			this.CopyFileHere(fileName, performMove, false);
		}
		
		/// <summary>
		/// Copies or moves a file to this directory.
		/// </summary>
		/// <param name="fileName">The name of the file to copy or move.</param>
		/// <param name="performMove">true to move the file, false to copy it.</param>
		/// <param name="keepDependency">true to copy the DependentUpon value of the file to the target if possible, false to discard the DependentUpon value.</param>
		public void CopyFileHere(string fileName, bool performMove, bool keepDependency)
		{
			string shortFileName = Path.GetFileName(fileName);
			string copiedFileName = Path.Combine(Directory, shortFileName);
			if (FileUtility.IsEqualFileName(fileName, copiedFileName))
				return;
			bool wasFileReplacement = false;
			if (File.Exists(copiedFileName)) {
				if (!FileService.FireFileReplacing(copiedFileName, false))
					return;
				if (AddExistingItemsToProject.ShowReplaceExistingFileDialog(null, copiedFileName, false) == AddExistingItemsToProject.ReplaceExistingFile.Yes) {
					wasFileReplacement = true;
					IViewContent viewContent = FileService.GetOpenFile(copiedFileName);
					if (viewContent != null) {
						viewContent.WorkbenchWindow.CloseWindow(true);
					}
				} else {
					// don't replace file
					return;
				}
			}
			
			FileProjectItem newItem = AddExistingItemsToProject.CopyFile(fileName, this, true);
			IProject sourceProject = SD.ProjectService.FindProjectContainingFile(FileName.Create(fileName));
			if (sourceProject != null) {
				string sourceDirectory = Path.GetDirectoryName(fileName);
				bool dependendElementsCopied = false;
				foreach (ProjectItem item in sourceProject.Items) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem == null)
						continue;
					if (newItem != null && FileUtility.IsEqualFileName(fileItem.FileName, fileName)) {
						fileItem.CopyMetadataTo(newItem);
						if (!keepDependency) {
							// Prevent the DependentUpon from being copied
							// because the referenced file is now in a different directory.
							newItem.DependentUpon = String.Empty;
						}
					}
					if (!string.Equals(fileItem.DependentUpon, shortFileName, StringComparison.OrdinalIgnoreCase))
						continue;
					string itemPath = Path.Combine(sourceProject.Directory, fileItem.VirtualName);
					if (!FileUtility.IsEqualFileName(sourceDirectory, Path.GetDirectoryName(itemPath)))
						continue;
					// this file is dependend on the file being copied/moved: copy it, too
					CopyFileHere(itemPath, performMove, true);
					dependendElementsCopied = true;
				}
				if (dependendElementsCopied)
					RecreateSubNodes();
			}
			if (performMove) {
				foreach (OpenedFile file in SD.FileService.OpenedFiles) {
					if (file.FileName != null &&
					    FileUtility.IsEqualFileName(file.FileName, fileName))
					{
						file.FileName = new FileName(copiedFileName);
					}
				}
				FileService.RemoveFile(fileName, false);
			}
			if (wasFileReplacement) {
				FileService.FireFileReplaced(copiedFileName, false);
			}
		}
		
		/// <summary>
		/// Copies or moves a file node (and the corresponding file, if applicable) to this directory,
		/// discarding its DependentUpon value.
		/// </summary>
		/// <param name="fileNode">The file node to copy or move.</param>
		/// <param name="performMove">true to move the file node, false to copy it.</param>
		public void CopyFileHere(FileNode node, bool performMove)
		{
			if (node.FileNodeStatus == FileNodeStatus.None) {
				AddExistingItemsToProject.CopyFile(node.FileName, this, false);
				if (performMove) {
					FileService.RemoveFile(node.FileName, false);
				}
			} else if (node.IsLink) {
				string relFileName = FileUtility.GetRelativePath(Project.Directory, node.FileName);
				FileNode fileNode = new FileNode(node.FileName, FileNodeStatus.InProject);
				FileProjectItem fileProjectItem = new FileProjectItem(Project, Project.GetDefaultItemType(node.FileName));
				fileProjectItem.Include = relFileName;
				fileProjectItem.SetEvaluatedMetadata("Link", Path.Combine(RelativePath, Path.GetFileName(node.FileName)));
				fileNode.ProjectItem = fileProjectItem;
				fileNode.InsertSorted(this);
				ProjectService.AddProjectItem(Project, fileProjectItem);
				if (performMove) {
					ProjectService.RemoveProjectItem(node.Project, node.ProjectItem);
					node.Remove();
				}
			} else {
				CopyFileHere(node.FileName, performMove);
			}
		}
		
		public override bool EnableCopy {
			get {
				if (IsEditing) {
					return false;
				}
				return true;
			}
		}
		public override void Copy()
		{
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, false));
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
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, true));
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
				} else {
					// Dragging a dependent file onto its parent directory
					// removes the dependency.
					FileProjectItem fpi = fileNode.ProjectItem as FileProjectItem;
					if (fpi != null && !String.IsNullOrEmpty(fpi.DependentUpon)) {
						return DragDropEffects.Move;
					}
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
				if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileNode fileNode = (FileNode)dataObject.GetData(typeof(FileNode));
					LoggingService.Debug("ProjectBrowser: Dragging file '" + fileNode.FileName + "' onto directory '" + this.Directory + "'");
					if (!FileUtility.IsEqualFileName(Directory, fileNode.FileName) && !FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(fileNode.FileName))
					    && !(fileNode.ProjectItem is FileProjectItem && FileUtility.IsEqualFileName(Directory, Path.GetDirectoryName(GetFullVirtualName((FileProjectItem)fileNode.ProjectItem))))) {
						LoggingService.Debug("-> Not in same directory, performing " + effect.ToString());
						CopyFileHere(fileNode, effect == DragDropEffects.Move);
					} else {
						// Dragging a dependent file onto its parent directory
						// removes the dependency.
						LoggingService.Debug("-> In same directory, removing dependency");
						((FileProjectItem)fileNode.ProjectItem).DependentUpon = String.Empty;
						fileNode.Remove();
						if (!File.Exists(fileNode.FileName)) {
							fileNode.FileNodeStatus = FileNodeStatus.Missing;
						} else {
							fileNode.FileNodeStatus = FileNodeStatus.InProject;
						}
						fileNode.InsertSorted(this);
					}
				} else if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					DirectoryNode directoryNode = (DirectoryNode)dataObject.GetData(typeof(DirectoryNode));
					CopyDirectoryHere(directoryNode, effect == DragDropEffects.Move);
				} else if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
					if (files != null) {
						foreach (string fileName in files) {
							if (System.IO.Directory.Exists(fileName)) {
								if (!FileUtility.IsBaseDirectory(fileName, Directory)) {
									CopyDirectoryHere(fileName, false);
								}
							} else {
								CopyFileHere(fileName, false);
							}
						}
					}
				}
				
				ProjectService.SaveSolution();
			} catch (Exception e) {
				MessageService.ShowException(e);
			}
		}
		
		static string GetFullVirtualName(FileProjectItem item)
		{
			if (Path.IsPathRooted(item.VirtualName)) {
				return item.VirtualName;
			} else if (item.Project != null) {
				return Path.Combine(item.Project.Directory, item.VirtualName);
			}
			return item.VirtualName;
		}
		#endregion
	}
}
