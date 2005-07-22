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
			
			FileService.FileRemoved  += FileRemoved;
			FileService.FileRenaming += FileRenamed;
			
			
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
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			ProjectBrowserPad pad = ProjectBrowserPad.Instance;
			if (pad == null) return;
			FileNode node = pad.ProjectBrowserControl.FindFileNode(e.FileName);
			if (node == null) return;
			OverlayIconManager.Enqueue(node);
		}
		
		void FileAdded(object sender, FileEventArgs e)
		{
//			Console.WriteLine("ADD : " + e.FileName);
			try {
				if (AddInOptions.AutomaticallyAddFiles) {
					SvnClient.Instance.Client.Add(Path.GetFullPath(e.FileName), false);
				}
			} catch (Exception ex) {
				MessageService.ShowError("File added exception: " + ex);
			}
		}
		
		void FileRemoved(object sender, FileEventArgs e)
		{
//			Console.WriteLine("REMOVE : " + e.FileName);
			try {
				if (AddInOptions.AutomaticallyDeleteFiles) {
					SvnClient.Instance.Client.Delete( new string [] {
					                                 	Path.GetFullPath(e.FileName)
					                                 }, true);
				}
			} catch (Exception ex) {
				MessageService.ShowError("File removed exception: " + ex);
			}
		}
		
		void FileRenamed(object sender, FileRenameEventArgs e)
		{
//			Console.WriteLine("RENAME : " + e.FileName);
			try {
				SvnClient.Instance.Client.Move(Path.GetFullPath(e.SourceFile),
				                               Revision.Unspecified, // TODO: Remove this line when upgrading to new NSvn version
				                               Path.GetFullPath(e.TargetFile),
				                               true
				                              );
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
	}
}
