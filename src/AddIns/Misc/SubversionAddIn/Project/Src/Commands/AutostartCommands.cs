
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

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;
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
			
			FileService.FileRemoved  += new FileEventHandler(FileRemoved);
			FileService.FileRenaming += new FileEventHandler(FileRenamed);
			
			
			projectService.FileRemovedFromProject += new FileEventHandler(FileRemoved);
			projectService.FileAddedToProject   += new FileEventHandler(FileAdded);
			
			
			FileUtility.FileSaved += new FileNameEventHandler(FileSaved);
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView));
			pbv.VisitRoot();
		}
		
		void FileAdded(object sender, FileEventArgs e)
		{
//			Console.WriteLine("ADD : " + e.FileName);
			try {
				if (AddInOptions.AutomaticallyAddFiles) {
					SvnClient.Instance.Client.Add(Path.GetFullPath(e.FileName), false);
				}
			} catch (Exception ex) {
				Console.WriteLine("File added exception: " + ex);
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
				Console.WriteLine("File removed exception: " + ex);
			}
		}
		
		void FileRenamed(object sender, FileEventArgs e)
		{
//			Console.WriteLine("RENAME : " + e.FileName);
			try {
				SvnClient.Instance.Client.Move(Path.GetFullPath(e.SourceFile), 
				                               Revision.Unspecified,
				                               Path.GetFullPath(e.TargetFile),
				                               true
				                              );
			} catch (Exception ex) {
				Console.WriteLine("File renamed exception: " + ex);
			}
		}
	}
}
