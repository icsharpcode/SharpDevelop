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
