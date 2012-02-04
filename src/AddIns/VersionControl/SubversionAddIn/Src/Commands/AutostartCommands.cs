// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Registers event handlers for file added, removed, renamed etc. and
	/// executes the appropriate Subversion commands.
	/// </summary>
	public sealed class RegisterEventsCommand : AbstractCommand
	{
		public override void Run()
		{
			FileService.FileRemoving += FileRemoving;
			FileService.FileRenaming += FileRenaming;
			FileService.FileCopying += FileCopying;
			FileService.FileCreated += FileCreated;
			
			ProjectService.SolutionCreated += SolutionCreated;
			ProjectService.ProjectCreated += ProjectCreated;
			
			FileUtility.FileSaved += FileSaved;
			AbstractProjectBrowserTreeNode.OnNewNode += TreeNodeCreated;
		}
		
		void TreeNodeCreated(object sender, TreeViewEventArgs e)
		{
			SolutionNode sn = e.Node as SolutionNode;
			if (sn != null) {
				if (CanBeVersionControlledDirectory(sn.Solution.Directory)) {
					OverlayIconManager.Enqueue(sn);
				}
			} else {
				DirectoryNode dn = e.Node as DirectoryNode;
				if (dn != null) {
					if (CanBeVersionControlledDirectory(dn.Directory)) {
						OverlayIconManager.Enqueue(dn);
					}
				} else {
					FileNode fn = e.Node as FileNode;
					if (fn != null) {
						if (CanBeVersionControlledFile(fn.FileName)) {
							OverlayIconManager.Enqueue(fn);
						}
					}
				}
			}
		}
		
		void SolutionCreated(object sender, SolutionEventArgs e)
		{
			if (!AddInOptions.AutomaticallyAddFiles) return;
			string solutionFileName = e.Solution.FileName;
			string solutionDirectory = e.Solution.Directory;
			
			if (!CanBeVersionControlledFile(solutionDirectory)) return;
			
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(solutionDirectory);
					if (status.TextStatus == StatusKind.Unversioned) {
						client.Add(solutionDirectory, Recurse.None);
					}
					status = client.SingleStatus(solutionFileName);
					if (status.TextStatus == StatusKind.Unversioned) {
						client.Add(solutionFileName, Recurse.None);
						client.AddToIgnoreList(solutionDirectory, Path.GetFileName(solutionFileName) + ".cache");
					}
				}
				foreach (IProject p in e.Solution.Projects) {
					ProjectCreated(null, new ProjectEventArgs(p));
				}
			} catch (SvnClientException ex) {
				MessageService.ShowError(ex.Message);
			} catch (Exception ex) {
				MessageService.ShowException(ex, "Solution add exception");
			}
		}
		
		
		void ProjectCreated(object sender, ProjectEventArgs e)
		{
			if (!AddInOptions.AutomaticallyAddFiles) return;
			if (!CanBeVersionControlledFile(e.Project.Directory)) return;
			
			string projectDir = Path.GetFullPath(e.Project.Directory);
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(projectDir);
					if (status.TextStatus != StatusKind.Unversioned)
						return;
					client.Add(projectDir, Recurse.None);
					if (FileUtility.IsBaseDirectory(Path.Combine(projectDir, "bin"), e.Project.OutputAssemblyFullPath)) {
						client.AddToIgnoreList(projectDir, "bin");
					}
					CompilableProject compilableProject = e.Project as CompilableProject;
					if (compilableProject != null) {
						if (FileUtility.IsBaseDirectory(Path.Combine(projectDir, "obj"), compilableProject.IntermediateOutputFullPath)) {
							client.AddToIgnoreList(projectDir, "obj");
						}
					}
					foreach (ProjectItem item in e.Project.Items) {
						FileProjectItem fileItem = item as FileProjectItem;
						if (fileItem != null) {
							if (FileUtility.IsBaseDirectory(projectDir, fileItem.FileName)) {
								AddFileWithParentDirectoriesToSvn(client, fileItem.FileName);
							}
						}
					}
					AddFileWithParentDirectoriesToSvn(client, e.Project.FileName);
				}
			} catch (SvnClientException ex) {
				MessageService.ShowError(ex.Message);
			} catch (Exception ex) {
				MessageService.ShowException(ex, "Project add exception");
			}
		}
		
		void AddFileWithParentDirectoriesToSvn(SvnClientWrapper client, string fileName)
		{
			if (!CanBeVersionControlledFile(fileName)) {
				AddFileWithParentDirectoriesToSvn(client, FileUtility.GetAbsolutePath(fileName, ".."));
			}
			Status status = client.SingleStatus(fileName);
			if (status.TextStatus != StatusKind.Unversioned)
				return;
			client.Add(fileName, Recurse.None);
		}
		
		internal static bool CanBeVersionControlledFile(string fileName)
		{
			return CanBeVersionControlledDirectory(Path.GetDirectoryName(fileName));
		}
		
		internal static bool CanBeVersionControlledDirectory(string directory)
		{
			return FindWorkingCopyRoot(directory) != null;
		}
		
		static string FindWorkingCopyRoot(string directory)
		{
			if (OverlayIconManager.SubversionDisabled)
				return null;
			try {
				if (!Path.IsPathRooted(directory))
					return null;
			} catch (ArgumentException) {
				return null;
			}
			DirectoryInfo info = new DirectoryInfo(directory);
			while (info != null) {
				if (Directory.Exists(Path.Combine(info.FullName, ".svn")))
					return info.FullName;
				info = info.Parent;
			}
			return null;
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			string fileName = e.FileName;
			if (!CanBeVersionControlledFile(fileName)) return;
			ClearStatusCacheAndEnqueueFile(fileName);
		}
		
		void ClearStatusCacheAndEnqueueFile(string fileName)
		{
			OverlayIconManager.ClearStatusCache();
			
			ProjectBrowserPad pad = ProjectBrowserPad.Instance;
			if (pad == null) return;
			FileNode node = pad.ProjectBrowserControl.FindFileNode(fileName);
			if (node == null) return;
			OverlayIconManager.Enqueue(node);
		}
		
		/// <summary>
		/// Adds the newly created file to the repository.
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">The name of the file and whether it is a directory or not.</param>
		void FileCreated(object sender, FileEventArgs e)
		{
			if (!AddInOptions.AutomaticallyAddFiles) return;
			if (!Path.IsPathRooted(e.FileName)) return;
			
			string fullName = Path.GetFullPath(e.FileName);
			if (!CanBeVersionControlledFile(fullName)) return;
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(fullName);
					switch (status.TextStatus) {
						case StatusKind.Unversioned:
						case StatusKind.Deleted:
							using (SharpDevelop.Gui.AsynchronousWaitDialog.ShowWaitDialog("svn add")) {
								client.Add(fullName, Recurse.None);
							}
							break;
					}
				}
			} catch (SvnClientException ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		void FileRemoving(object sender, FileCancelEventArgs e)
		{
			if (e.Cancel) return;
			string fullName = Path.GetFullPath(e.FileName);
			if (!CanBeVersionControlledFile(fullName)) return;
			
			if (e.IsDirectory) {
				
				// show "cannot delete directories" message even if
				// AutomaticallyDeleteFiles (see below) is off!
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(fullName);
					switch (status.TextStatus) {
						case StatusKind.None:
						case StatusKind.Unversioned:
						case StatusKind.Ignored:
							break;
						default:
							// must be done using the subversion client, even if
							// AutomaticallyDeleteFiles is off, because we don't want to corrupt the
							// working copy
							e.OperationAlreadyDone = true;
							try {
								client.Delete(new string[] { fullName }, false);
							} catch (SvnClientException ex) {
								LoggingService.Warn("SVN Error" + ex);
								LoggingService.Warn(ex);
								
								if (ex.IsKnownError(KnownError.CannotDeleteFileWithLocalModifications)
								    || ex.IsKnownError(KnownError.CannotDeleteFileNotUnderVersionControl))
								{
									if (MessageService.ShowCustomDialog("${res:AddIns.Subversion.DeleteDirectory}",
									                                    StringParser.Parse("${res:AddIns.Subversion.ErrorDelete}:\n",
									                                                       new StringTagPair("File", fullName)) +
									                                    ex.Message, 0, 1,
									                                    "${res:AddIns.Subversion.ForceDelete}", "${res:Global.CancelButtonText}")
									    == 0)
									{
										try {
											client.Delete(new string[] { fullName }, true);
										} catch (SvnClientException ex2) {
											e.Cancel = true;
											MessageService.ShowError(ex2.Message);
										}
									} else {
										e.Cancel = true;
									}
								} else {
									e.Cancel = true;
									MessageService.ShowError(ex.Message);
								}
							}
							break;
					}
				}
				return;
			}
			// not a directory, but a file:
			
			if (!AddInOptions.AutomaticallyDeleteFiles) return;
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(fullName);
					switch (status.TextStatus) {
						case StatusKind.None:
						case StatusKind.Unversioned:
						case StatusKind.Ignored:
						case StatusKind.Deleted:
							return; // nothing to do
						case StatusKind.Normal:
							// remove without problem
							break;
						case StatusKind.Modified:
						case StatusKind.Replaced:
							if (MessageService.AskQuestion("${res:AddIns.Subversion.RevertLocalModifications}")) {
								// modified files cannot be deleted, so we need to revert the changes first
								client.Revert(new string[] { fullName }, e.IsDirectory ? Recurse.Full : Recurse.None);
							} else {
								e.Cancel = true;
								return;
							}
							break;
						case StatusKind.Added:
							if (status.Copied) {
								if (!MessageService.AskQuestion("${res:AddIns.Subversion.RemoveMovedFile}")) {
									e.Cancel = true;
									return;
								}
							}
							client.Revert(new string[] { fullName }, e.IsDirectory ? Recurse.Full : Recurse.None);
							return;
						default:
							MessageService.ShowErrorFormatted("${res:AddIns.Subversion.CannotRemoveError}", status.TextStatus.ToString());
							e.Cancel = true;
							return;
					}
					e.OperationAlreadyDone = true;
					client.Delete(new string [] { fullName }, true);
				}
			} catch (Exception ex) {
				MessageService.ShowError("File removed exception: " + ex);
			}
		}
		
		void FileCopying(object sender, FileRenamingEventArgs e)
		{
			if (e.Cancel) return;
			if (!AddInOptions.AutomaticallyRenameFiles) return;
			string fullSource = Path.GetFullPath(e.SourceFile);
			if (!CanBeVersionControlledFile(fullSource)) return;
			string fullTarget = Path.GetFullPath(e.TargetFile);
			if (!CanBeVersionControlledFile(fullTarget)) return;
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(fullSource);
					switch (status.TextStatus) {
						case StatusKind.Unversioned:
						case StatusKind.None:
							return; // nothing to do
						case StatusKind.Normal:
						case StatusKind.Modified:
						case StatusKind.Replaced:
						case StatusKind.Added:
							// copy without problem
							break;
						default:
							MessageService.ShowErrorFormatted("${res:AddIns.Subversion.CannotCopyError}", status.TextStatus.ToString());
							e.Cancel = true;
							return;
					}
					e.OperationAlreadyDone = true;
					client.Copy(fullSource, fullTarget);
				}
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
		
		void FileRenaming(object sender, FileRenamingEventArgs e)
		{
			if (e.Cancel) return;
			if (!AddInOptions.AutomaticallyRenameFiles) return;
			string fullSource = Path.GetFullPath(e.SourceFile);
			if (!CanBeVersionControlledFile(fullSource)) return;
			try {
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					SvnMessageView.HandleNotifications(client);
					
					Status status = client.SingleStatus(fullSource);
					switch (status.TextStatus) {
						case StatusKind.Unversioned:
						case StatusKind.None:
						case StatusKind.Ignored:
							return; // nothing to do
						case StatusKind.Normal:
						case StatusKind.Modified:
						case StatusKind.Replaced:
						case StatusKind.Added:
							// rename without problem
							break;
						default:
							MessageService.ShowErrorFormatted("${res:AddIns.Subversion.CannotMoveError}", status.TextStatus.ToString());
							e.Cancel = true;
							return;
					}
					e.OperationAlreadyDone = true;
					client.Move(fullSource,
					            Path.GetFullPath(e.TargetFile),
					            true
					           );
				}
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
	}
}
