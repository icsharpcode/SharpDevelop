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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using System.Windows.Forms;

namespace ICSharpCode.GitAddIn
{
	public class RegisterEventsCommand : AbstractCommand
	{
		public override void Run()
		{
			FileService.FileCreated += (sender, e) => {
				AddFile(e.FileName);
			};
			FileService.FileCopied += (sender, e) => {
				AddFile(e.TargetFile);
			};
			FileService.FileRemoved += (sender, e) => {
				RemoveFile(e.FileName);
			};
			FileService.FileRenamed += (sender, e) => {
				RenameFile(e.SourceFile, e.TargetFile);
			};
			FileUtility.FileSaved += (sender, e) => {
				ClearStatusCacheAndEnqueueFile(e.FileName);
			};
			AbstractProjectBrowserTreeNode.OnNewNode += TreeNodeCreated;
		}
		
		async void AddFile(string fileName)
		{
			if (!AddInOptions.AutomaticallyAddFiles) return;
			await Git.AddAsync(fileName);
			ClearStatusCacheAndEnqueueFile(fileName);
		}
		
		async void RemoveFile(string fileName)
		{
			if (!AddInOptions.AutomaticallyDeleteFiles) return;
			if (GitStatusCache.GetFileStatus(fileName) == GitStatus.Added) {
				await Git.RemoveAsync(fileName, true);
				ClearStatusCacheAndEnqueueFile(fileName);
			}
		}
		
		async void RenameFile(string sourceFileName, string targetFileName)
		{
			if (AddInOptions.AutomaticallyAddFiles)
				await Git.AddAsync(targetFileName);
			ClearStatusCacheAndEnqueueFile(targetFileName);
			RemoveFile(sourceFileName);
		}
		
		void TreeNodeCreated(object sender, TreeViewEventArgs e)
		{
			SolutionNode sn = e.Node as SolutionNode;
			if (sn != null) {
				GitStatusCache.ClearCachedStatus(sn.Solution.FileName);
				OverlayIconManager.Enqueue(sn);
			} else {
				DirectoryNode dn = e.Node as DirectoryNode;
				if (dn != null) {
					OverlayIconManager.Enqueue(dn);
				} else {
					FileNode fn = e.Node as FileNode;
					if (fn != null) {
						OverlayIconManager.Enqueue(fn);
					}
				}
			}
		}
		
		void ClearStatusCacheAndEnqueueFile(string fileName)
		{
			GitStatusCache.ClearCachedStatus(fileName);
			
			ProjectBrowserPad pad = ProjectBrowserPad.Instance;
			if (pad == null) return;
			FileNode node = pad.ProjectBrowserControl.FindFileNode(fileName);
			if (node == null) return;
			OverlayIconManager.EnqueueParents(node);
		}
	}
}
