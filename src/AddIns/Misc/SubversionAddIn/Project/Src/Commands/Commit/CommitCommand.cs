/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 23.11.2004
 * Time: 15:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
using ICSharpCode.Svn.Gui;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of CommitCommand.
	/// </summary>
	public class CommitCommand : AbstractMenuCommand
	{
		public CommitCommand()
		{
		}
		
		void DoCommit()
		{
		}
		
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				IProject project = node.Project;
				if (project == null) {
					return;
				}
				
				string fileName = projectService.GetFileName(project);
				if (fileName == null) {
					return;
				}
				fileName = Path.GetDirectoryName(fileName);
				
				using (CommitDialog commitDialog = new CommitDialog()) {
					commitDialog.LogMessage = AddInOptions.DefaultLogMessage;
					if (commitDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						SvnClient.Instance.LogMessage = commitDialog.LogMessage;
						SvnClient.Instance.OperationStart("Commit", new ThreadStart(DoCommit));
					}
				}
			}
		}
	}
}
