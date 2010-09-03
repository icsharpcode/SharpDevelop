// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// The panel in the "history" secondary viewcontent. Contains a tabcontol.
	/// </summary>
	public class HistoryViewPanel : Panel
	{
		string fileName;
		
		InfoPanel infoPanel;
		
		public HistoryViewPanel(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.fileName = FileUtility.NormalizePath(fileName);
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
			/*
			TabControl mainTab = new TabControl();
			mainTab.Dock       = DockStyle.Fill;
			mainTab.Alignment  = TabAlignment.Bottom;
			
			Controls.Add(mainTab);
			
			TabPage infoTabPage = new TabPage("Info");
			infoPanel = new InfoPanel(viewContent);
			infoTabPage.Controls.Add(infoPanel);
			mainTab.TabPages.Add(infoTabPage);
			
			infoPanel.Dock = DockStyle.Fill;
			
			
			TabPage diffTabPage = new TabPage("Diff");
			diffPanel = new DiffPanel(viewContent);
			diffTabPage.Controls.Add(diffPanel);
			mainTab.TabPages.Add(diffTabPage);
			
			diffPanel.Dock  = DockStyle.Fill;
			 */
			
			/*
			TabPage conflictTabPage = new TabPage("Conflicts");
			Label todoLabel = new Label();
			todoLabel.Text = "TODO :)";
			conflictTabPage.Controls.Add(todoLabel);
			mainTab.TabPages.Add(conflictTabPage);
			 */
			
			infoPanel = new InfoPanel(fileName);
			infoPanel.Dock = DockStyle.Fill;
			Controls.Add(infoPanel);
			
			Thread logMessageThread = new Thread(new ThreadStart(GetLogMessages));
			logMessageThread.Name = "svnLogMessage";
			logMessageThread.IsBackground = true;
			logMessageThread.Start();
		}
		
		void GetLogMessages()
		{
			try {
				LoggingService.Info("SVN: Get log of " + fileName);
				if (File.Exists(fileName)) {
					using (SvnClientWrapper client = new SvnClientWrapper()) {
						client.AllowInteractiveAuthorization();
						client.Log(new string[] { fileName },
						           Revision.Head,          // Revision start
						           Revision.FromNumber(1), // Revision end
						           int.MaxValue,           // Limit
						           false,                  // bool discoverChangePath
						           false,                  // bool strictNodeHistory
						           ReceiveLogMessage);
					}
				}
			} catch (Exception ex) {
				// if exceptions aren't caught here, they force SD to exit
				if (ex is SvnClientException || ex is System.Runtime.InteropServices.SEHException) {
					LoggingService.Warn(ex);
					WorkbenchSingleton.SafeThreadAsyncCall(infoPanel.ShowError, ex);
				} else {
					MessageService.ShowException(ex);
				}
			}
		}
		
		void ReceiveLogMessage(LogMessage logMessage)
		{
			if (infoPanel != null) {
				WorkbenchSingleton.SafeThreadAsyncCall(infoPanel.AddLogMessage, logMessage);
			}
//			if (diffPanel != null) {
//				WorkbenchSingleton.SafeThreadAsyncCall(diffPanel.AddLogMessage, logMessage);
//			}
		}
	}
}
