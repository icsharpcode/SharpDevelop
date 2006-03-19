// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddExistingItemsToProject : AbstractMenuCommand
	{
		enum ReplaceExistingFile {
			Yes = 0,
			YesToAll = 1,
			No = 2,
			Cancel = 3
		}
		
		int GetFileFilterIndex(IProject project, string[] fileFilters)
		{
			if (project != null) {
				LanguageBindingDescriptor languageCodon = LanguageBindingService.GetCodonPerLanguageName(project.Language);
				for (int i = 0; i < fileFilters.Length; ++i) {
					for (int j = 0; j < languageCodon.Supportedextensions.Length; ++j) {
						if (fileFilters[i].ToUpperInvariant().IndexOf(languageCodon.Supportedextensions[j].ToUpperInvariant()) >= 0) {
							return i + 1;
						}
					}
				}
			}
			return 0;
		}
		
		public static void CopyDirectory(string directoryName, DirectoryNode node, bool includeInProject)
		{
			directoryName = Path.GetFullPath(directoryName);
			string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(directoryName));
			LoggingService.Debug("Copy " + directoryName + " to " + copiedFileName);
			if (!FileUtility.IsEqualFileName(directoryName, copiedFileName)) {
				if (includeInProject && ProjectService.OpenSolution != null) {
					// get ProjectItems in source directory
					foreach (IProject project in ProjectService.OpenSolution.Projects) {
						if (!FileUtility.IsBaseDirectory(project.Directory, directoryName))
							continue;
						LoggingService.Debug("Searching for child items in " + project.Name);
						foreach (ProjectItem item in project.Items.ToArray()) {
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
								fileItem.CopyExtraPropertiesTo(newItem);
								if (fileItem.IsLink) {
									string newVirtualFullName = FileUtility.RenameBaseDirectory(virtualFullName, directoryName, copiedFileName);
									fileItem.Properties["Link"] = FileUtility.GetRelativePath(node.Project.Directory, newVirtualFullName);
								}
								node.Project.Items.Add(newItem);
							}
						}
					}
				}
				
				FileUtility.DeepCopy(directoryName, copiedFileName, true);
				DirectoryNode newNode = new DirectoryNode(copiedFileName);
				newNode.AddTo(node);
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
				File.Copy(fileName, copiedFileName, true);
			} 
			if (includeInProject) {
				FileNode fileNode;
				foreach (TreeNode childNode in node.AllNodes) {
					if (childNode is FileNode) {
						fileNode = (FileNode)childNode;
						if (FileUtility.IsEqualFileName(fileNode.FileName, copiedFileName)) {
							if (fileNode.FileNodeStatus == FileNodeStatus.Missing) {
								fileNode.FileNodeStatus = FileNodeStatus.InProject;
							}
							return fileNode.ProjectItem as FileProjectItem;
						}
					}
				}
				fileNode = new FileNode(copiedFileName);
				fileNode.AddTo(node);
				return IncludeFileInProject.IncludeFileNode(fileNode);
			}
			return null;
		}
		
		public static IEnumerable<string> FindAdditionalFiles(string fileName)
		{
			List<string> list = new List<string>();
			StringParser.Properties["Extension"] = Path.GetExtension(fileName);
			string prefix = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
			foreach (string ext in AddInTree.BuildItems("/SharpDevelop/Workbench/DependentFileExtensions", null, false)) {
				if (File.Exists(prefix + ext))
					list.Add(prefix + ext);
			}
			return list;
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
			if (node == null) {
				return;
			}
			node.Expanding();
			node.Expand();
			
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				string[] fileFilters  = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
				
				fdiag.FilterIndex     = GetFileFilterIndex(node.Project, fileFilters);
				fdiag.Filter          = String.Join("|", fileFilters);
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				fdiag.Title = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles}");
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
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
						int res = MessageService.ShowCustomDialog(fdiag.Title, "${res:ProjectComponent.ContextMenu.AddExistingFiles.Question}",
						                                          0, 2,
						                                          "${res:ProjectComponent.ContextMenu.AddExistingFiles.Copy}",
						                                          "${res:ProjectComponent.ContextMenu.AddExistingFiles.Link}",
						                                          "${res:Global.CancelButtonText}");
						if (res == 1) {
							foreach (KeyValuePair<string, string> pair in fileNames) {
								string fileName = pair.Key;
								string relFileName = FileUtility.GetRelativePath(node.Project.Directory, fileName);
								FileNode fileNode = new FileNode(fileName, FileNodeStatus.InProject);
								FileProjectItem fileProjectItem = new FileProjectItem(node.Project, IncludeFileInProject.GetDefaultItemType(node.Project, fileName));
								fileProjectItem.Include = relFileName;
								fileProjectItem.Properties.Set("Link", Path.Combine(node.RelativePath, Path.GetFileName(fileName)));
								fileProjectItem.DependentUpon = pair.Value;
								fileNode.ProjectItem = fileProjectItem;
								fileNode.AddTo(node);
								ProjectService.AddProjectItem(node.Project, fileProjectItem);
							}
							node.Project.Save();
							if (addedDependentFiles)
								node.RecreateSubNodes();
							return;
						}
						if (res == 2) {
							return;
						}
					}
					bool replaceAll = false;
					foreach (KeyValuePair<string, string> pair in fileNames) {
						copiedFileName = Path.Combine(node.Directory, Path.GetFileName(pair.Key));
						if (!replaceAll && File.Exists(copiedFileName) && !FileUtility.IsEqualFileName(pair.Key, copiedFileName)) {
							ReplaceExistingFile res = (ReplaceExistingFile)MessageService.ShowCustomDialog(fdiag.Title, "A file with the name '" + Path.GetFileName(pair.Key) + "' already exists. Do you want to replace it?",
						    	0, 3,
						    	"${res:Global.Yes}",
								"Yes to All",
								"${res:Global.No}",
								"${res:Global.CancelButtonText}");
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
							item.DependentUpon = pair.Value;
						}
					}
					node.Project.Save();
					if (addedDependentFiles)
						node.RecreateSubNodes();
				}
			}
		}
	}
	
	public class AddNewItemsToProject : AbstractMenuCommand
	{
		
		FileProjectItem CreateNewFile(DirectoryNode upper, string fileName)
		{
			upper.Expanding();
			
			FileNode fileNode = new FileNode(fileName, FileNodeStatus.InProject);
			fileNode.AddTo(upper);
			fileNode.EnsureVisible();
			return IncludeFileInProject.IncludeFileNode(fileNode);
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
			if (node == null) {
				return;
			}
			node.Expand();
			node.Expanding();
			
			using (NewFileDialog nfd = new NewFileDialog(node.Directory)) {
				if (nfd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					bool additionalProperties = false;
					foreach (KeyValuePair<string, PropertyGroup> createdFile in nfd.CreatedFiles) {
						FileProjectItem item = CreateNewFile(node, createdFile.Key);
						if (createdFile.Value.PropertyCount > 0) {
							additionalProperties = true;
							item.Properties.Merge(createdFile.Value);
						}
					}
					if (additionalProperties) {
						node.Project.Save();
						node.RecreateSubNodes();
					}
				}
			}
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
			
			DirectoryNode directoryNode = new DirectoryNode(directoryName, FileNodeStatus.InProject);
			directoryNode.AddTo(upper);
			
			IncludeFileInProject.IncludeDirectoryNode(directoryNode, false);
			return directoryNode;
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
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
			Directory.CreateDirectory(node.Directory);
			IncludeFileInProject.IncludeDirectoryNode(node, false);
		}
	}
}
