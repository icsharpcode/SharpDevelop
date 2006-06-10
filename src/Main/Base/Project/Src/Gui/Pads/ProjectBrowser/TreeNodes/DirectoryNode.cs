// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;

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
		
		public static IDataObject CreateDataObject(FileNode node, bool performMove)
		{
			return new DataObject(typeof(FileNode).ToString(), new FileOperationClipboardObject(node.FileName, performMove));
		}
		
		public static IDataObject CreateDataObject(SolutionItemNode node, bool performMove)
		{
			return new DataObject(typeof(SolutionItemNode).ToString(),
			                      new FileOperationClipboardObject(node.FileName, performMove));
		}
		
		public static IDataObject CreateDataObject(DirectoryNode node, bool performMove)
		{
			return new DataObject(typeof(DirectoryNode).ToString(),
			                      new FileOperationClipboardObject(node.Directory, performMove));
		}
	}
	
	public enum SpecialFolder {
		None,
		AppDesigner,
		WebReference,
		WebReferencesFolder
	}
	
	public class DirectoryNodeFactory
	{
		DirectoryNodeFactory()
		{
		}
		
		public static DirectoryNode CreateDirectoryNode(TreeNode parent, IProject project, string directory)
		{
			DirectoryNode node = new DirectoryNode(directory);
			if (directory == Path.Combine(project.Directory, project.AppDesignerFolder)) {
				node.SpecialFolder = SpecialFolder.AppDesigner;
			} else if (DirectoryNode.IsWebReferencesFolder(project, directory)) {
				node = new WebReferencesFolderNode(directory);
			} else if (parent != null && parent is WebReferencesFolderNode) {
				node = new WebReferenceNode(directory);
			}
			return node;
		}
		
		public static DirectoryNode CreateDirectoryNode(ProjectItem item, FileNodeStatus status)
		{
			DirectoryNode node;
			if (item is WebReferencesProjectItem) {
				node = new WebReferencesFolderNode((WebReferencesProjectItem)item);
				node.FileNodeStatus = status;
			} else {
				node = new DirectoryNode(item.FileName.Trim('\\', '/'), status);
				node.ProjectItem = item;
			}
			return node;
		}
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
						case SpecialFolder.WebReferencesFolder:
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
			canLabelEdit = true;
		}
		
		public DirectoryNode(string directory) : this(directory, FileNodeStatus.None)
		{
			sortOrder = 1;
			canLabelEdit = true;
		}
		CustomNode removeMe = null;
		public DirectoryNode(string directory, FileNodeStatus fileNodeStatus)
		{
			sortOrder = 1;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/FolderNode";
			this.Directory = directory;
			this.fileNodeStatus = fileNodeStatus;
			
			removeMe = new CustomNode();
			removeMe.AddTo(this);
			
			SetIcon();
			canLabelEdit = true;
		}
		
		/// <summary>
		/// Determines if the specified <paramref name="folder"/> is a
		/// web reference folder in the specified <paramref name="project"/>.
		/// </summary>
		/// <param name="folder">The full folder path.</param>
		public static bool IsWebReferencesFolder(IProject project, string folder)
		{
			foreach (ProjectItem item in project.Items) {
				if (item.ItemType == ItemType.WebReferences) {
					if (FileUtility.IsEqualFileName(Path.Combine(project.Directory, item.Include), folder)) {
						return true;
					}
				}
			}
			return false;
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
				= new Dictionary<string, FileNode>(StringComparer.InvariantCultureIgnoreCase);
			Dictionary<FileNode, string> dependendFileDictionary = new Dictionary<FileNode, string>();
			Dictionary<string, DirectoryNode> directoryNodeList = new Dictionary<string, DirectoryNode>(StringComparer.InvariantCultureIgnoreCase);
			
			// Add files found in file system
			
			if (System.IO.Directory.Exists(Directory)) {
				foreach (string subDirectory in System.IO.Directory.GetDirectories(Directory)) {
					if (Path.GetFileName(subDirectory) != ".svn") {
						DirectoryNode newDirectoryNode = DirectoryNodeFactory.CreateDirectoryNode(this, Project, subDirectory);
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
				if (!string.Equals(virtualName, relativeDirectoryPath + fileName, StringComparison.InvariantCultureIgnoreCase)) {
					AddParentFolder(virtualName, relativeDirectoryPath, directoryNodeList);
					continue;
				}
				
				if (item.ItemType == ItemType.Folder || item.ItemType == ItemType.WebReferences) {
					DirectoryNode node;
					if (directoryNodeList.TryGetValue(fileName, out node)) {
						if (node.FileNodeStatus == FileNodeStatus.None) {
							node.FileNodeStatus = FileNodeStatus.InProject;
						}
						node.ProjectItem = item;
					} else {
						node = DirectoryNodeFactory.CreateDirectoryNode(item, FileNodeStatus.Missing);
						node.AddTo(this);
						directoryNodeList[fileName] = node;
					}
				} else {
					FileNode node;
					if (fileItem.IsLink) {
						node = new FileNode(fileItem.FileName, FileNodeStatus.InProject);
						node.AddTo(this);
						fileNodeDictionary[fileName] = node;
					} else {
						if (fileNodeDictionary.TryGetValue(fileName, out node)) {
							if (node.FileNodeStatus == FileNodeStatus.None) {
								node.FileNodeStatus = FileNodeStatus.InProject;
							}
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

		protected void BaseInitialize()
		{
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
					if (node.FileNodeStatus == FileNodeStatus.None) {
						node.FileNodeStatus = FileNodeStatus.InProject;
					}
				} else {
					node = new DirectoryNode(Path.Combine(Directory, subFolderName), FileNodeStatus.Missing);
					node.AddTo(this);
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
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			if (!FileService.CheckDirectoryName(newName)) {
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
				IDataObject dataObject = ClipboardWrapper.GetDataObject();
				if (dataObject == null) {
					return false;
				}
				if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
					return true;
				}
				if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(FileNode).ToString());
					return File.Exists(clipboardObject.FileName);
				}
				if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					FileOperationClipboardObject clipboardObject = (FileOperationClipboardObject)dataObject.GetData(typeof(DirectoryNode).ToString());
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
			IDataObject dataObject = ClipboardWrapper.GetDataObject();
			
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
		
		public void CopyDirectoryHere(string fileName, bool performMove)
		{
			AddExistingItemsToProject.CopyDirectory(fileName, this, true);
			if (performMove) {
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.FileName != null &&
					    FileUtility.IsBaseDirectory(fileName, content.FileName))
					{
						content.FileName = FileUtility.RenameBaseDirectory(content.FileName, fileName, Path.Combine(this.directory, Path.GetFileName(fileName)));
					}
				}
				FileService.RemoveFile(fileName, true);
			}
		}
		
		public void CopyDirectoryHere(DirectoryNode node, bool performMove)
		{
			CopyDirectoryHere(node.Directory, performMove);
		}
		
		public void CopyFileHere(string fileName, bool performMove)
		{
			string shortFileName = Path.GetFileName(fileName);
			string copiedFileName = Path.Combine(Directory, shortFileName);
			if (FileUtility.IsEqualFileName(fileName, copiedFileName))
				return;
			
			FileProjectItem newItem = AddExistingItemsToProject.CopyFile(fileName, this, true);
			IProject sourceProject = Solution.FindProjectContainingFile(fileName);
			if (sourceProject != null) {
				string sourceDirectory = Path.GetDirectoryName(fileName);
				bool dependendElementsCopied = false;
				foreach (ProjectItem item in sourceProject.Items.ToArray()) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem == null)
						continue;
					if (newItem != null && FileUtility.IsEqualFileName(fileItem.FileName, fileName)) {
						fileItem.CopyExtraPropertiesTo(newItem);
					}
					if (!string.Equals(fileItem.DependentUpon, shortFileName, StringComparison.OrdinalIgnoreCase))
						continue;
					string itemPath = Path.Combine(sourceProject.Directory, fileItem.VirtualName);
					if (!FileUtility.IsEqualFileName(sourceDirectory, Path.GetDirectoryName(itemPath)))
						continue;
					// this file is dependend on the file being copied/moved: copy it, too
					CopyFileHere(itemPath, performMove);
					dependendElementsCopied = true;
				}
				if (dependendElementsCopied)
					RecreateSubNodes();
			}
			if (performMove) {
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.FileName != null &&
					    FileUtility.IsEqualFileName(content.FileName, fileName))
					{
						content.FileName  = copiedFileName;
						content.TitleName = shortFileName;
					}
				}
				FileService.RemoveFile(fileName, false);
			}
		}
		
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
				FileProjectItem fileProjectItem = new FileProjectItem(Project, IncludeFileInProject.GetDefaultItemType(Project, node.FileName));
				fileProjectItem.Include = relFileName;
				fileProjectItem.Properties.Set("Link", Path.Combine(RelativePath, Path.GetFileName(node.FileName)));
				fileNode.ProjectItem = fileProjectItem;
				fileNode.AddTo(this);
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
				if (dataObject.GetDataPresent(typeof(FileNode))) {
					FileNode fileNode = (FileNode)dataObject.GetData(typeof(FileNode));
					CopyFileHere(fileNode, effect == DragDropEffects.Move);
				} else if (dataObject.GetDataPresent(typeof(DirectoryNode))) {
					DirectoryNode directoryNode = (DirectoryNode)dataObject.GetData(typeof(DirectoryNode));
					CopyDirectoryHere(directoryNode, effect == DragDropEffects.Move);
				} else if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
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
				}
				
				ProjectService.SaveSolution();
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		#endregion
	}
}
