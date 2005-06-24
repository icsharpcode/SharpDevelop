
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
	/// Description of ProjectBrowserCommands.
	/// </summary>
	public class UpdateCommand : AbstractMenuCommand
	{
		string fileName;
		
		void DoUpdateCommand()
		{
			SvnClient.Instance.Client.Update(Directory.Exists(fileName) ? fileName : Path.GetDirectoryName(fileName), Revision.Working, true);
		}
		
		public override void Run()
		{
			Console.WriteLine(Owner);
			/*
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				fileName = node.FileName;
				if (fileName == null) {
					return;
				}
				SvnClient.Instance.OperationStart("Update", new ThreadStart(DoUpdateCommand));
				SvnClient.Instance.WaitForOperationEnd();
				
				if (AddInOptions.AutomaticallyReloadProject) {
					
					projectService.ReloadCombine();
				}
			}
			*/
		}
	}
	
	/// <summary>
	/// Description of ProjectBrowserCommands.
	/// </summary>
	public class RevertCommand : AbstractMenuCommand
	{
		string fileName;
		
		void DoRevertCommand()
		{
			SvnClient.Instance.Client.Revert(new string[] { Path.GetDirectoryName(fileName) }, true);
		}
		
		public override void Run()
		{
			/*ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				IProject project = node.Project;
				if (project == null) {
					return;
				}
				
				fileName = projectService.GetFileName(project);
				if (fileName == null) {
					return;
				}
				SvnClient.Instance.OperationStart("Revert", new ThreadStart(DoRevertCommand));
				SvnClient.Instance.WaitForOperationEnd();
				projectService.ReloadCombine();
			}*/
		}
	}
	
	/// <summary>
	/// Description of CreatePatchCommand
	/// </summary>
	public class CreatePatchCommand : AbstractMenuCommand
	{
		string fileName;
		string output;
		
		void DoCreatePatchCommand()
		{
			try {
				MemoryStream outStream = new MemoryStream();
				MemoryStream errStream = new MemoryStream();
				
				SvnClient.Instance.Client.Diff(new string [] {} ,
				                fileName,
					            Revision.Committed,
					            fileName,
					            Revision.Working,
					            true,
					            false,
					            true,
					            outStream,
					            errStream);
				output = Encoding.Default.GetString(outStream.ToArray());
			} catch (Exception e) {
				Console.WriteLine("Exception while patch generation : " + e);
			} finally {
				if (output == null) {
					output = "";
				}
			}
		}
		
		public override void Run()
		{
			/*ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				IProject project = node.Project;
				if (project == null) {
					return;
				}
				
				fileName = projectService.GetFileName(project);
				if (fileName == null) {
					return;
				}
				fileName = Path.GetDirectoryName(fileName);
				SvnClient.Instance.OperationStart("CreatePatch", new ThreadStart(DoCreatePatchCommand));
				
				while (output == null) {
					Application.DoEvents();
				}
				FileService.NewFile("a.patch", "txt", output);
				
			}*/
		}
	}
}
