using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class FileRenameTreeNodeVisitor : ProjectBrowserTreeNodeVisitor
	{
		string oldName;
		string newName;
		
		public FileRenameTreeNodeVisitor(string oldName, string newName)
		{
			this.oldName = oldName;
			this.newName = newName;
		}
		
		public override object Visit(SolutionItemNode solutionItemNode, object data)
		{
			if (FileUtility.IsEqualFile(oldName, solutionItemNode.FileName)) {
				solutionItemNode.Text = Path.GetFileName(newName);
			}
			solutionItemNode.AcceptChildren(this, data);
			return data;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			directoryNode.Directory = FileUtility.RenameBaseDirectory(directoryNode.Directory, oldName, newName);
			directoryNode.AcceptChildren(this, data);
			return data;
		}
		
		public override object Visit(FileNode fileNode, object data)
		{
			fileNode.FileName = FileUtility.RenameBaseDirectory(fileNode.FileName, oldName, newName);
			fileNode.AcceptChildren(this, data);
			return data;
		}
	}
}
