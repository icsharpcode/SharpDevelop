// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddExistingItemsToProject : AbstractMenuCommand
	{
		public enum ReplaceExistingFile {
			Yes = 0,
			YesToAll = 1,
			No = 2,
			Cancel = 3
		}
		
		public static ReplaceExistingFile ShowReplaceExistingFileDialog(string caption, string fileName, bool replacingMultiple)
		{
			if (caption == null)
				caption = "${res:ProjectComponent.ContextMenu.AddExistingFiles.ReplaceExistingFile.Title}";
			string text = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles.ReplaceExistingFile}", new StringTagPair("FileName", fileName));
			if (replacingMultiple) {
				return (ReplaceExistingFile)
					MessageService.ShowCustomDialog(caption, text,
					                                0, 3,
					                                "${res:Global.Yes}",
					                                "${res:Global.YesToAll}",
					                                "${res:Global.No}",
					                                "${res:Global.CancelButtonText}");
			} else {
				return MessageService.AskQuestion(text, caption)
					? ReplaceExistingFile.Yes : ReplaceExistingFile.No;
			}
		}
		
		int GetFileFilterIndex(IProject project, IList<FileFilterDescriptor> fileFilters)
		{
			if (project != null) {
				ProjectBindingDescriptor projectCodon = ProjectBindingService.GetCodonPerLanguageName(project.Language);
				if (projectCodon != null) {
					for (int i = 0; i < fileFilters.Count; ++i) {
						for (int j = 0; j < projectCodon.CodeFileExtensions.Length; ++j) {
							if (fileFilters[i].ContainsExtension(projectCodon.CodeFileExtensions[j])) {
								return i + 1;
							}
						}
					}
				}
			}
			return 0;
		}
		
		public static void CopyDirectory(string directoryName, DirectoryNode node, bool includeInProject)
		{
			directoryName = FileUtility.NormalizePath(directoryName);
			string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(directoryName));
			LoggingService.Debug("Copy " + directoryName + " to " + copiedFileName);
			if (!FileUtility.IsEqualFileName(directoryName, copiedFileName)) {
				if (includeInProject && ProjectService.OpenSolution != null) {
					// get ProjectItems in source directory
					foreach (IProject project in ProjectService.OpenSolution.Projects) {
						if (!FileUtility.IsBaseDirectory(project.Directory, directoryName))
							continue;
						LoggingService.Debug("Searching for child items in " + project.Name);
						foreach (ProjectItem item in project.Items) {
							FileProjectItem fileItem = item as FileProjectItem;
							if (fileItem == null)
								continue;
							string virtualFullName = Path.Combine(project.Directory, fileItem.VirtualName);
							if (FileUtility.IsBaseDirectory(directoryName, virtualFullName)) {
								if (item.ItemType == ItemType.Folder && FileUtility.IsEqualFileName(directoryName, virtualFullName)) {
									continue;
								}
								LoggingService.Debug("Found file " + virtualFullName);
								FileProjectItem newItem = new FileProjectItem(node.Project, fileItem.ItemType);
								if (FileUtility.IsBaseDirectory(directoryName, fileItem.FileName)) {
									newItem.FileName = FileUtility.RenameBaseDirectory(fileItem.FileName, directoryName, copiedFileName);
								} else {
									newItem.FileName = fileItem.FileName;
								}
								fileItem.CopyMetadataTo(newItem);
								if (fileItem.IsLink) {
									string newVirtualFullName = FileUtility.RenameBaseDirectory(virtualFullName, directoryName, copiedFileName);
									fileItem.SetEvaluatedMetadata("Link", FileUtility.GetRelativePath(node.Project.Directory, newVirtualFullName));
								}
								ProjectService.AddProjectItem(node.Project, newItem);
							}
						}
					}
				}
				
				FileService.CopyFile(directoryName, copiedFileName, true, false);
				DirectoryNode newNode = new DirectoryNode(copiedFileName);
				newNode.InsertSorted(node);
				if (includeInProject) {
					IncludeFileInProject.IncludeDirectoryNode(newNode, false);
				}
				newNode.Expanding();
			} else if (includeInProject) {
				foreach (TreeNode childNode in node.Nodes) {
					if (childNode is DirectoryNode) {
						DirectoryNode directoryNode = (DirectoryNode)childNode;
						if (FileUtility.IsEqualFileName(directoryNode.Directory, copiedFileName)) {
							IncludeFileInProject.IncludeDirectoryNode(directoryNode, true);
						}
					}
				}
			}
		}
		
		public static FileProjectItem CopyFile(string fileName, DirectoryNode node, bool includeInProject)
		{
			string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(fileName));
			if (!FileUtility.IsEqualFileName(fileName, copiedFileName)) {
				FileService.CopyFile(fileName, copiedFileName, false, true);
			}
			if (includeInProject) {
				FileNode fileNode;
				foreach (TreeNode childNode in node.AllNodes) {
					if (childNode is FileNode) {
						fileNode = (FileNode)childNode;
						if (FileUtility.IsEqualFileName(fileNode.FileName, copiedFileName)) {
							if (fileNode.FileNodeStatus == FileNodeStatus.Missing) {
								fileNode.FileNodeStatus = FileNodeStatus.InProject;
							} else if (fileNode.FileNodeStatus == FileNodeStatus.None) {
								return IncludeFileInProject.IncludeFileNode(fileNode);
							}
							return fileNode.ProjectItem as FileProjectItem;
						}
					}
				}
				fileNode = new FileNode(copiedFileName);
				fileNode.InsertSorted(node);
				return IncludeFileInProject.IncludeFileNode(fileNode);
			}
			return null;
		}
		
		public static IEnumerable<string> FindAdditionalFiles(string fileName)
		{
			List<string> list = new List<string>();
			// HACK: find a different way to support .Designer.XYZ
			StringParserPropertyContainer.FileCreation["Extension"] = Path.GetExtension(fileName);
			string prefix = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
			foreach (string ext in AddInTree.BuildItems<string>("/SharpDevelop/Workbench/DependentFileExtensions", null, false)) {
				if (File.Exists(prefix + ext))
					list.Add(prefix + ext);
			}
			return list;
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.BringToFront();
			this.AddExistingItems();
		}
		
		protected IEnumerable<FileProjectItem> AddExistingItems()
		{
			DirectoryNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedDirectoryNode;
			if (node == null) {
				return null;
			}
			node.Expanding();
			node.Expand();
			
			List<FileProjectItem> addedItems = new List<FileProjectItem>();
			
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension = true;
				var fileFilters    = ProjectService.GetFileFilters();
				
				fdiag.InitialDirectory = node.Directory;
				fdiag.FilterIndex     = GetFileFilterIndex(node.Project, fileFilters);
				fdiag.Filter          = String.Join("|", fileFilters);
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				fdiag.Title = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles}");
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					List<KeyValuePair<string, string>> fileNames = new List<KeyValuePair<string, string>>(fdiag.FileNames.Length);
					foreach (string fileName in fdiag.FileNames) {
						fileNames.Add(new KeyValuePair<string, string>(fileName, ""));
					}
					bool addedDependentFiles = false;
					foreach (string fileName in fdiag.FileNames) {
						foreach (string additionalFile in FindAdditionalFiles(fileName)) {
							if (!fileNames.Exists(delegate(KeyValuePair<string, string> pair) {
							                      	return FileUtility.IsEqualFileName(pair.Key, additionalFile);
							                      }))
							{
								addedDependentFiles = true;
								fileNames.Add(new KeyValuePair<string, string>(additionalFile, Path.GetFileName(fileName)));
							}
						}
					}
					
					
					
					string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(fileNames[0].Key));
					if (!FileUtility.IsEqualFileName(fileNames[0].Key, copiedFileName)) {
						int res = MessageService.ShowCustomDialog(
							fdiag.Title, "${res:ProjectComponent.ContextMenu.AddExistingFiles.Question}",
							0, 2,
							"${res:ProjectComponent.ContextMenu.AddExistingFiles.Copy}",
							"${res:ProjectComponent.ContextMenu.AddExistingFiles.Link}",
							"${res:Global.CancelButtonText}");
						if (res == 1) {
							// Link
							foreach (KeyValuePair<string, string> pair in fileNames) {
								string fileName = pair.Key;
								string relFileName = FileUtility.GetRelativePath(node.Project.Directory, fileName);
								FileNode fileNode = new FileNode(fileName, FileNodeStatus.InProject);
								FileProjectItem fileProjectItem = new FileProjectItem(node.Project, node.Project.GetDefaultItemType(fileName), relFileName);
								fileProjectItem.SetEvaluatedMetadata("Link", Path.Combine(node.RelativePath, Path.GetFileName(fileName)));
								fileProjectItem.DependentUpon = pair.Value;
								addedItems.Add(fileProjectItem);
								fileNode.ProjectItem = fileProjectItem;
								fileNode.InsertSorted(node);
								ProjectService.AddProjectItem(node.Project, fileProjectItem);
							}
							node.Project.Save();
							if (addedDependentFiles)
								node.RecreateSubNodes();
							return addedItems.AsReadOnly();
						}
						if (res == 2) {
							// Cancel
							return addedItems.AsReadOnly();
						}
						// only continue for res==0 (Copy)
					}
					bool replaceAll = false;
					foreach (KeyValuePair<string, string> pair in fileNames) {
						copiedFileName = Path.Combine(node.Directory, Path.GetFileName(pair.Key));
						if (!replaceAll && File.Exists(copiedFileName) && !FileUtility.IsEqualFileName(pair.Key, copiedFileName)) {
							ReplaceExistingFile res = ShowReplaceExistingFileDialog(fdiag.Title, Path.GetFileName(pair.Key), true);
							if (res == ReplaceExistingFile.YesToAll) {
								replaceAll = true;
							} else if (res == ReplaceExistingFile.No) {
								continue;
							} else if (res == ReplaceExistingFile.Cancel) {
								break;
							}
						}
						FileProjectItem item = CopyFile(pair.Key, node, true);
						if (item != null) {
							addedItems.Add(item);
							item.DependentUpon = pair.Value;
						}
					}
					node.Project.Save();
					if (addedDependentFiles)
						node.RecreateSubNodes();
				}
			}
			
			return addedItems.AsReadOnly();
		}
	}
	
	public class AddExistingFolderToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserPad.Instance.BringToFront();
			DirectoryNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedDirectoryNode;
			if (node == null) {
				return;
			}
			node.Expanding();
			node.Expand();
			
			using (FolderBrowserDialog dlg = new FolderBrowserDialog()) {
				dlg.SelectedPath = node.Directory;
				dlg.ShowNewFolderButton = false;
				if (dlg.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					string folderName = dlg.SelectedPath;
					string copiedFolderName = Path.Combine(node.Directory, Path.GetFileName(folderName));
					if (!FileUtility.IsEqualFileName(folderName, copiedFolderName)) {
						if (FileUtility.IsBaseDirectory(folderName, node.Directory)) {
							MessageService.ShowError("Cannot copy " + folderName + " to " + copiedFolderName);
							return;
						}
						if (Directory.Exists(copiedFolderName)) {
							MessageService.ShowError("Cannot copy " + folderName + " to " + copiedFolderName + ": target already exists.");
							return;
						}
						int res = MessageService.ShowCustomDialog(
							"${res:ProjectComponent.ContextMenu.ExistingFolder}",
							"${res:ProjectComponent.ContextMenu.ExistingFolder.CopyQuestion}",
							0, 1,
							"${res:ProjectComponent.ContextMenu.AddExistingFiles.Copy}",
							"${res:Global.CancelButtonText}");
						if (res != 0)
							return;
						if (!FileService.CopyFile(folderName, copiedFolderName, true, false))
							return;
					}
					// ugly HACK to get IncludeDirectoryNode to work properly
					AbstractProjectBrowserTreeNode.ShowAll = true;
					try {
						node.RecreateSubNodes();
						DirectoryNode newNode = node.AllNodes.OfType<DirectoryNode>()
							.FirstOrDefault(dir=>FileUtility.IsEqualFileName(copiedFolderName, dir.Directory));
						if (newNode != null) {
							newNode.Expanding();
							IncludeFileInProject.IncludeDirectoryNode(newNode, true);
						}
					} finally {
						AbstractProjectBrowserTreeNode.ShowAll = false;
					}
					node.RecreateSubNodes();
				}
			}
		}
	}
	
	/// <summary>
	/// Menu item that display the NewFileDialog dialog and adds it to the solution.
	/// </summary>
	/// <seealso cref="NewFileDialog"/>
	/// <exception cref="NullReferenceException">
	/// Thrown if the selected node does not have an ancestor that is a DirectoryNode.
	/// </exception>
	public class AddNewItemsToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserPad.Instance.BringToFront();
			this.AddNewItems();
		}
		
		protected IEnumerable<FileProjectItem> AddNewItems()
		{
			DirectoryNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedDirectoryNode;
			if (node == null) {
				return null;
			}
			node.Expand();
			node.Expanding();
			
			List<FileProjectItem> addedItems = new List<FileProjectItem>();
			
			using (NewFileDialog nfd = new NewFileDialog(node.Directory)) {
				if (nfd.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					bool additionalProperties = false;
					foreach (KeyValuePair<string, FileDescriptionTemplate> createdFile in nfd.CreatedFiles) {
						FileProjectItem item = node.AddNewFile(createdFile.Key);
						addedItems.Add(item);
						
						if (createdFile.Value.SetProjectItemProperties(item)) {
							additionalProperties = true;
						}
					}
					if (additionalProperties) {
						node.Project.Save();
						node.RecreateSubNodes();
					}
				}
			}
			
			return addedItems.AsReadOnly();
		}
	}
	
	public class AddNewFolderToProject : AbstractMenuCommand
	{
		string GenerateValidDirectoryName(string inDirectory)
		{
			string directoryName = Path.Combine(inDirectory, ResourceService.GetString("ProjectComponent.NewFolderString"));
			
			if (Directory.Exists(directoryName)) {
				int index = 1;
				while (Directory.Exists(directoryName + index)) {
					index++;
				}
				return directoryName + index;
			}
			return directoryName;
		}
		
		DirectoryNode CreateNewDirectory(DirectoryNode upper, string directoryName)
		{
			upper.Expanding();
			Directory.CreateDirectory(directoryName);
			FileService.FireFileCreated(directoryName, true);
			
			DirectoryNode directoryNode = new DirectoryNode(directoryName, FileNodeStatus.InProject);
			directoryNode.InsertSorted(upper);
			
			IncludeFileInProject.IncludeDirectoryNode(directoryNode, false);
			return directoryNode;
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.BringToFront();
			
			DirectoryNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedDirectoryNode;
			if (node == null) {
				return;
			}
			node.Expanding();
			string newDirectoryName = GenerateValidDirectoryName(node.Directory);
			DirectoryNode newDirectoryNode = CreateNewDirectory(node, newDirectoryName);
			ProjectBrowserPad.Instance.StartLabelEdit(newDirectoryNode);
		}
	}
	
	public class CreateMissingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node != null) {
				Directory.CreateDirectory(node.Directory);
				IncludeFileInProject.IncludeDirectoryNode(node, false);
			}
		}
	}
}
