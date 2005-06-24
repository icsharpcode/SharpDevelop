
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
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			// TODO: Reimplement
			//ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView));
			//pbv.VisitRoot();
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
				SvnClient.Instance.Client.Delete( new string [] {
					Path.GetFullPath(e.FileName)
				}, true);
			} catch (Exception ex) {
				MessageService.ShowError("File removed exception: " + ex);
			}
		}
		
		void FileRenamed(object sender, FileRenameEventArgs e)
		{
//			Console.WriteLine("RENAME : " + e.FileName);
			try {
				SvnClient.Instance.Client.Move(Path.GetFullPath(e.SourceFile), 
				                               Revision.Unspecified,
				                               Path.GetFullPath(e.TargetFile),
				                               true
				                              );
			} catch (Exception ex) {
				MessageService.ShowError("File renamed exception: " + ex);
			}
		}
	}
}
