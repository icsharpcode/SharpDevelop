// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;

namespace FSharpBinding
{
	public class FSharpProjectNode : ProjectNode
	{
		public FSharpProjectNode(IProject project) : base(project)
		{
		}
		
		public void AddParentFolder(string virtualName, string relativeDirectoryPath, Dictionary<string, DirectoryNode> directoryNodeList)
		{
			if ((relativeDirectoryPath.Length == 0)
			    || (string.Compare(virtualName, 0, relativeDirectoryPath, 0, relativeDirectoryPath.Length, StringComparison.InvariantCultureIgnoreCase) == 0))
			{
				int pos = virtualName.IndexOf('/', relativeDirectoryPath.Length + 1);
				if (pos > 0) {
					string subFolderName = virtualName.Substring(relativeDirectoryPath.Length, pos - relativeDirectoryPath.Length);
					DirectoryNode node;
					if (directoryNodeList.TryGetValue(subFolderName, out node)) {
						if (node.FileNodeStatus == FileNodeStatus.None) {
							node.FileNodeStatus = FileNodeStatus.InProject;
						}
					} else {
						node = new DirectoryNode(Path.Combine(this.Directory, subFolderName), FileNodeStatus.Missing);
						node.AddTo(this);
						directoryNodeList[subFolderName] = node;
					}
				}
			}
		}
		
		protected override void Initialize()
		{
			Dictionary<string, FileNode> fileNodeDictionary = new Dictionary<string, FileNode>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
			Dictionary<string, DirectoryNode> directoryNodeList = new Dictionary<string, DirectoryNode>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
			string relativeDirectoryPath = (this.RelativePath.Length <= 0) ? string.Empty : (this.RelativePath.Replace('\\', '/') + "/");
			
			foreach (FileProjectItem item in this.Project.Items.OfType<FileProjectItem>()) {
				string virtualName = item.VirtualName.Replace('\\', '/');
				if (virtualName.EndsWith("/", StringComparison.Ordinal))
					virtualName = virtualName.Substring(0, virtualName.Length - 1);
				string fileName = Path.GetFileName(virtualName);
				if (!string.Equals(virtualName, relativeDirectoryPath + fileName, StringComparison.InvariantCultureIgnoreCase)) {
					this.AddParentFolder(virtualName, relativeDirectoryPath, directoryNodeList);
					// continue;
				}
				
				if (item.ItemType == ItemType.Folder || item.ItemType == ItemType.WebReferences) {
					DirectoryNode newDirectoryNode = DirectoryNodeFactory.CreateDirectoryNode(this, this.Project, fileName);
					if (!System.IO.Directory.Exists(item.FileName)) {
						newDirectoryNode.FileNodeStatus = FileNodeStatus.Missing;
					}
					newDirectoryNode.ProjectItem = item;
					newDirectoryNode.AddTo(this);
					directoryNodeList[fileName] = newDirectoryNode;
				} else {
					FileNode fileNode = new FileNode(item.FileName);
					if (!File.Exists(item.FileName)) {
						fileNode.FileNodeStatus = FileNodeStatus.Missing;
					}
					fileNode.ProjectItem = item;
					fileNodeDictionary[fileName] = fileNode;
					fileNode.AddTo(this);
				}
			}
			
			// Add files found in file system
			if (System.IO.Directory.Exists(this.Directory)) {
				foreach (string subDirectory in System.IO.Directory.GetDirectories(this.Directory)) {
					string filename = Path.GetFileName(subDirectory);
					if (filename != ".svn") {
						DirectoryNode node;
						if (directoryNodeList.TryGetValue(filename, out node)) {
							if (node.FileNodeStatus == FileNodeStatus.None)
								node.FileNodeStatus = FileNodeStatus.InProject;
						} else {
							node = DirectoryNodeFactory.CreateDirectoryNode(this, this.Project, subDirectory);
							node.AddTo(this);
						}
					}
				}
				
				foreach (string fullpath in System.IO.Directory.GetFiles(this.Directory)) {
					string file = Path.GetFileName(fullpath);
					FileNode node;
					if (fileNodeDictionary.TryGetValue(file, out node)) {
						if (node.FileNodeStatus == FileNodeStatus.None)
							node.FileNodeStatus = FileNodeStatus.InProject;
					} else {
						node = new FileNode(file);
						node.AddTo(this);
					}
				}
			}
		}
	}
}
