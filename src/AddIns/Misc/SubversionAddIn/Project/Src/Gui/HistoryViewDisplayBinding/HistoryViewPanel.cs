/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 22.11.2004
 * Time: 10:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
		
		ContentPanel contentPanel;
		InfoPanel infoPanel;
		DiffPanel diffPanel;
		
		public HistoryViewPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			TabControl mainTab = new TabControl();
			mainTab.Dock       = DockStyle.Fill;
			mainTab.Alignment  = TabAlignment.Bottom;
			
			TabPage contentTabPage = new TabPage("Content");
			contentPanel = new ContentPanel(viewContent);
			contentPanel.Dock = DockStyle.Fill;
			contentTabPage.Controls.Add(contentPanel);
			mainTab.TabPages.Add(contentTabPage);
			
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
			if (File.Exists(fileName)) {
				Client client = new Client();
				client.Log(new string[] { fileName},
				           Revision.FromNumber(1), // Revision start
				           Revision.Working,       // Revision end
				           false,                  // bool discoverChangePath
				           false,                  // bool strictNodeHistory
				           new LogMessageReceiver(ReceiveLogMessage));
			}
		}
		
		void ReceiveLogMessage(LogMessage logMessage)
		{
			WorkbenchSingleton.SafeThreadCall(infoPanel, "AddLogMessage", logMessage);
			WorkbenchSingleton.SafeThreadCall(contentPanel, "AddLogMessage", logMessage);
			WorkbenchSingleton.SafeThreadCall(diffPanel, "AddLogMessage", logMessage);
		}
	}
}
