// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of AutostartCommands.
	/// </summary>
	public class RegisterEventsCommand : AbstractCommand
	{
		public override void Run()
		{
			FileService.FileRemoving += FileRemoving;
			FileService.FileRenaming += FileRenaming;
			
			//projectService.FileRemovedFromProject += FileRemoved;
			//projectService.FileAddedToProject   += FileAdded);
			
			FileUtility.FileSaved += new FileNameEventHandler(FileSaved);
			AbstractProjectBrowserTreeNode.AfterNodeInitialize += TreeNodeInitialized;
		}
		
		SvnProjectBrowserVisitor visitor = new SvnProjectBrowserVisitor();
		
		void TreeNodeInitialized(object sender, TreeViewEventArgs e)
		{
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			node.AcceptVisitor(visitor, null);
		}
		
		bool CanBeVersionControlled(string fileName)
		{
			string svnDir = Path.Combine(Path.GetDirectoryName(fileName), ".svn");
			return Directory.Exists(svnDir);
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			ProjectBrowserPad pad = ProjectBrowserPad.Instance;
			if (pad == null) return;
			string fileName = e.FileName;
			if (!CanBeVersionControlled(fileName)) return;
			FileNode node = pad.ProjectBrowserControl.FindFileNode(fileName);
			if (node == null) return;
			OverlayIconManager.Enqueue(node);
		}
		
		void FileAdded(object sender, FileEventArgs e)
		{
			try {
				if (AddInOptions.AutomaticallyAddFiles) {
					if (!CanBeVersionControlled(e.FileName)) return;
					SvnClient.Instance.Client.Add(Path.GetFullPath(e.FileName), false);
				}
			} catch (Exception ex) {
				MessageService.ShowError("File added exception: " + ex);
			}
		}
		
		void FileRemoving(object sender, FileCancelEventArgs e)
		{
			if (e.Cancel) return;
			if (e.IsDirectory) return;
			if (!AddInOptions.AutomaticallyDeleteFiles) return;
			string fullName = Path.GetFullPath(e.FileName);
			if (!CanBeVersionControlled(fullName)) return;
			try {
				Status status = SvnClient.Instance.Client.SingleStatus(fullName);
				switch (status.TextStatus) {
					case StatusKind.None:
					case StatusKind.Unversioned:
						return; // nothing to do
					case StatusKind.Normal:
						// remove without problem
						break;
					case StatusKind.Modified:
					case StatusKind.Replaced:
						MessageService.ShowError("The file has local modifications. Do you really want to remove it?");
						e.Cancel = true;
						break;
					case StatusKind.Added:
						if (status.Copied) {
							MessageService.ShowError("The file has just been moved to this location, do you really want to remove it?");
							e.Cancel = true;
							return;
						}
						SvnClient.Instance.Client.Revert(new string[] { fullName }, e.IsDirectory);
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
			string fullSource = Path.GetFullPath(e.SourceFile);
			if (!CanBeVersionControlled(fullSource)) return;
			try {
				Status status = SvnClient.Instance.Client.SingleStatus(fullSource);
				switch (status.TextStatus) {
					case StatusKind.Unversioned:
					case StatusKind.None:
						return; // nothing to do
					case StatusKind.Normal:
					case StatusKind.Modified:
						// rename without problem
						break;
					case StatusKind.Added:
					case StatusKind.Replaced:
						if (status.Copied) {
							MessageService.ShowError("The file was moved/copied and cannot be renamed without losing it's history.");
							e.Cancel = true;
						} else if (e.IsDirectory) {
							goto default;
						} else {
							SvnClient.Instance.Client.Revert(new string[] { fullSource }, false);
							FileService.FileRenamed += new AutoAddAfterRenameHelper(e).Renamed;
						}
						return;
					default:
						MessageService.ShowError("The file/directory cannot be renamed because it is in subversion status '" + status.TextStatus + "'.");
						e.Cancel = true;
						return;
				}
				SvnClient.Instance.Client.Move(fullSource,
				                               Revision.Unspecified, // TODO: Remove this line when upgrading to new NSvn version
				                               Path.GetFullPath(e.TargetFile),
				                               true
				                              );
				e.OperationAlreadyDone = true;
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
		
		class AutoAddAfterRenameHelper
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
				SvnClient.Instance.Client.Add(e.TargetFile, false);
			}
		}
	}
}
