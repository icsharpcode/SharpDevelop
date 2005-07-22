// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of HistoryViewPanel.
	/// </summary>
	public class HistoryViewPanel : Panel
	{
		IViewContent viewContent;
		
		InfoPanel infoPanel;
		DiffPanel diffPanel;
		
		public HistoryViewPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible && infoPanel == null) {
				Initialize();
			}
		}
		
		void Initialize()
		{
			TabControl mainTab = new TabControl();
			mainTab.Dock       = DockStyle.Fill;
			mainTab.Alignment  = TabAlignment.Bottom;
			
			
			TabPage infoTabPage = new TabPage("Info");
			infoPanel = new InfoPanel(viewContent);
			infoPanel.Dock = DockStyle.Fill;
			infoTabPage.Controls.Add(infoPanel);
			mainTab.TabPages.Add(infoTabPage);
			
			
			TabPage diffTabPage = new TabPage("Diff");
			diffPanel = new DiffPanel(viewContent);
			diffPanel.Dock  = DockStyle.Fill;
			diffTabPage.Controls.Add(diffPanel);
			mainTab.TabPages.Add(diffTabPage);
			
			TabPage conflictTabPage = new TabPage("Conflicts");
			Label todoLabel = new Label();
			todoLabel.Text = "TODO :)";
			conflictTabPage.Controls.Add(todoLabel);
			mainTab.TabPages.Add(conflictTabPage);
			
			Controls.Add(mainTab);
			
			Thread logMessageThread = new Thread(new ThreadStart(GetLogMessages));
			logMessageThread.IsBackground = true;
			logMessageThread.Start();
		}
		
		void GetLogMessages()
		{
			string fileName = Path.GetFullPath(viewContent.FileName);
			Console.WriteLine("Get log of " + fileName);
			if (File.Exists(fileName)) {
				Client client = SvnClient.Instance.Client;
				client.Log(new string[] { fileName},
				           Revision.Head,          // Revision start
				           Revision.FromNumber(1), // Revision end
				           false,                  // bool discoverChangePath
				           false,                  // bool strictNodeHistory
				           new LogMessageReceiver(ReceiveLogMessage));
			}
		}
		
		void ReceiveLogMessage(LogMessage logMessage)
		{
			WorkbenchSingleton.SafeThreadCall(infoPanel, "AddLogMessage", logMessage);
			WorkbenchSingleton.SafeThreadCall(diffPanel, "AddLogMessage", logMessage);
		}
	}
}
