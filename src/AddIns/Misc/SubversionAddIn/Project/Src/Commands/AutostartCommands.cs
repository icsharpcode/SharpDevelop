// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Registers event handlers for file added, removed, renamed etc. and
	/// executes the appropriate Subversion commands.
	/// </summary>
	public sealed class RegisterEventsCommand : AbstractCommand
	{
		const int CannotDeleteFileWithLocalModifications = 195006;
		const int CannotDeleteFileNotUnderVersionControl = 200005;
		
		public override void Run()
		{
			FileService.FileRemoving += FileRemoving;
			FileService.FileRenaming += FileRenaming;
			FileService.FileCreated += FileCreated;
			
			FileUtility.FileSaved += new FileNameEventHandler(FileSaved);
			AbstractProjectBrowserTreeNode.AfterNodeInitialize += TreeNodeInitialized;
		}
		
		SvnProjectBrowserVisitor visitor = new SvnProjectBrowserVisitor();
		
		void TreeNodeInitialized(object sender, TreeViewEventArgs e)
		{
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			node.AcceptVisitor(visitor, null);
		}
		
		internal static bool CanBeVersionControlledFile(string fileName)
		{
			return CanBeVersionControlledDirectory(Path.GetDirectoryName(fileName));
		}
		
		internal static bool CanBeVersionControlledDirectory(string directory)
		{
			return Directory.Exists(Path.Combine(directory, ".svn")) || Directory.Exists(Path.Combine(directory, "_svn"));
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			ProjectBrowserPad pad = ProjectBrowserPad.Instance;
			if (pad == null) return;
			string fileName = e.FileName;
			if (!CanBeVersionControlledFile(fileName)) return;
			FileNode node = pad.ProjectBrowserControl.FindFileNode(fileName);
			if (node == null) return;
			OverlayIconManager.Enqueue(node);
			SubversionStateCondition.ResetCache();
		}
		
		void FileCreated(object sender, FileEventArgs e)
		{
			if (e.IsDirectory) return;
			if (!AddInOptions.AutomaticallyAddFiles) return;
			if (!Path.IsPathRooted(e.FileName)) return;
			
			string fullName = Path.GetFullPath(e.FileName);
			if (!CanBeVersionControlledFile(fullName)) return;
			try {
				Status status = SvnClient.Instance.Client.SingleStatus(fullName);
				switch (status.TextStatus) {
					case StatusKind.Unversioned:
					case StatusKind.Deleted:
						if (SvnClient.Instance.Client.IsIgnored(fullName))
							return;
						SvnClient.Instance.Client.Add(fullName, Recurse.None);
						break;
				}
			} catch (Exception ex) {
				MessageService.ShowError("File add exception: " + ex);
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
				Status status = SvnClient.Instance.Client.SingleStatus(fullName);
				switch (status.TextStatus) {
					case StatusKind.None:
					case StatusKind.Unversioned:
						break;
					default:
						// must be done using the subversion client, even if
						// AutomaticallyDeleteFiles is off, because we don't want to corrupt the
						// working copy
						e.OperationAlreadyDone = true;
						try {
							SvnClient.Instance.Client.Delete(new string[] { fullName }, false);
						} catch (SvnClientException ex) {
							LoggingService.Warn("SVN Error code " + ex.ErrorCode);
							LoggingService.Warn(ex);
							
							if (ex.ErrorCode == CannotDeleteFileWithLocalModifications
							    || ex.ErrorCode == CannotDeleteFileNotUnderVersionControl)
							{
								if (MessageService.ShowCustomDialog("Delete directory",
								                                    "Error deleting " + fullName + ":\n" +
								                                    ex.Message, 0, 1,
								                                    "Force delete", "${res:Global.CancelButtonText}")
								    == 0)
								{
									try {
										SvnClient.Instance.Client.Delete(new string[] { fullName }, true);
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
				return;
			}
			// not a directory, but a file:
			
			if (!AddInOptions.AutomaticallyDeleteFiles) return;
			try {
				Status status = SvnClient.Instance.Client.SingleStatus(fullName);
				switch (status.TextStatus) {
					case StatusKind.None:
					case StatusKind.Unversioned:
					case StatusKind.Deleted:
						return; // nothing to do
					case StatusKind.Normal:
						// remove without problem
						break;
					case StatusKind.Modified:
					case StatusKind.Replaced:
						if (MessageService.AskQuestion("The file has local modifications. Do you really want to remove it?")) {
							// modified files cannot be deleted, so we need to revert the changes first
							SvnClient.Instance.Client.Revert(new string[] { fullName }, e.IsDirectory ? Recurse.Full : Recurse.None);
						} else {
							e.Cancel = true;
							return;
						}
						break;
					case StatusKind.Added:
						if (status.Copied) {
							if (!MessageService.AskQuestion("The file has just been moved to this location, do you really want to remove it?")) {
								e.Cancel = true;
								return;
							}
						}
						SvnClient.Instance.Client.Revert(new string[] { fullName }, e.IsDirectory ? Recurse.Full : Recurse.None);
						return;
					default:
						MessageService.ShowError("The file/directory cannot be removed because it is in subversion status '" + status.TextStatus + "'.");
						e.Cancel = true;
						return;
				}
				SvnClient.Instance.Client.Delete(new string [] { fullName }, true);
				e.OperationAlreadyDone = true;
			} catch (Exception ex) {
				MessageService.ShowError("File removed exception: " + ex);
			}
		}
		
		void FileRenaming(object sender, FileRenamingEventArgs e)
		{
			if (e.Cancel) return;
			if (!AddInOptions.AutomaticallyRenameFiles) return;
			string fullSource = Path.GetFullPath(e.SourceFile);
			if (!CanBeVersionControlledFile(fullSource)) return;
			try {
				Status status = SvnClient.Instance.Client.SingleStatus(fullSource);
				switch (status.TextStatus) {
					case StatusKind.Unversioned:
					case StatusKind.None:
						return; // nothing to do
					case StatusKind.Normal:
					case StatusKind.Modified:
					case StatusKind.Replaced:
						// rename without problem
						break;
					case StatusKind.Added:
						if (status.Copied) {
							MessageService.ShowError("The file was moved/copied and cannot be renamed without losing it's history.");
							e.Cancel = true;
						} else if (e.IsDirectory) {
							goto default;
						} else {
							SvnClient.Instance.Client.Revert(new string[] { fullSource }, Recurse.None);
							FileService.FileRenamed += new AutoAddAfterRenameHelper(e).Renamed;
						}
						return;
					default:
						MessageService.ShowError("The file/directory cannot be renamed because it is in subversion status '" + status.TextStatus + "'.");
						e.Cancel = true;
						return;
				}
				SvnClient.Instance.Client.Move(fullSource,
				                               Path.GetFullPath(e.TargetFile),
				                               true
				                              );
				e.OperationAlreadyDone = true;
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
		
		sealed class AutoAddAfterRenameHelper
		{
			FileRenamingEventArgs args;
			
			public AutoAddAfterRenameHelper(FileRenamingEventArgs args) {
				this.args = args;
			}
			
			public void Renamed(object sender, FileRenameEventArgs e)
			{
				FileService.FileRenamed -= Renamed;
				if (args.Cancel || args.OperationAlreadyDone)
					return;
				if (args.SourceFile != e.SourceFile || args.TargetFile != e.TargetFile)
					return;
				SvnClient.Instance.Client.Add(e.TargetFile, Recurse.None);
			}
		}
	}
}
