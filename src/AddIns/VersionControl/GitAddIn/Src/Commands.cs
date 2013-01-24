// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.GitAddIn
{
	public abstract class GitCommand : AbstractMenuCommand
	{
		protected abstract void Run(string filename, Action callback);
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null) {
				string nodeFileName = null;
				if (node is DirectoryNode) {
					nodeFileName = ((DirectoryNode)node).Directory;
				} else if (node is FileNode) {
					nodeFileName =  ((FileNode)node).FileName;
				} else if (node is SolutionNode) {
					nodeFileName = ((SolutionNode)node).Solution.Directory;
				}
				if (nodeFileName != null) {
					List<OpenedFile> unsavedFiles = new List<OpenedFile>();
					foreach (OpenedFile file in FileService.OpenedFiles) {
						if (file.IsDirty && !file.IsUntitled) {
							if (string.IsNullOrEmpty(file.FileName)) continue;
							if (FileUtility.IsUrl(file.FileName)) continue;
							if (FileUtility.IsBaseDirectory(nodeFileName, file.FileName)) {
								unsavedFiles.Add(file);
							}
						}
					}
					if (unsavedFiles.Count > 0) {
						if (MessageService.ShowCustomDialog(
							MessageService.DefaultMessageBoxTitle,
							"The version control operation would affect files with unsaved modifications.\n" +
							"You have to save those files before running the operation.",
							0, 1,
							"Save files", "Cancel")
						    == 0)
						{
							// Save
							foreach (OpenedFile file in unsavedFiles) {
								ICSharpCode.SharpDevelop.Commands.SaveFile.Save(file);
							}
						} else {
							// Cancel
							return;
						}
					}
					// now run the actual operation:
					Run(nodeFileName, AfterCommand(nodeFileName, node));
				}
			}
		}
		
		Action AfterCommand(string nodeFileName, AbstractProjectBrowserTreeNode node)
		{
			return delegate {
				WorkbenchSingleton.AssertMainThread();
				// and then refresh the project browser:
				GitStatusCache.ClearCachedStatus(nodeFileName);
				OverlayIconManager.EnqueueRecursive(node);
				OverlayIconManager.EnqueueParents(node);
			};
		}
	}
	
	public class GitCommitCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Commit(filename, callback);
		}
	}
	
	public class GitDiffCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Diff(filename, callback);
		}
	}
	
	public class GitLogCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Log(filename, callback);
		}
	}
}
