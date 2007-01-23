// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of InfoPanel.
	/// </summary>
	public class InfoPanel : BaseSharpDevelopUserControl
	{
		IViewContent viewContent;
		ListView revisionList, changesList;
		
		public InfoPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.InfoPanel.xfrm"));
			
			revisionList = Get<ListView>("revision");
			changesList  = Get<ListView>("changes");
			revisionList.SelectedIndexChanged += RevisionListViewSelectionChanged;
			ControlDictionary["commentRichTextBox"].Enabled = false;
			ControlDictionary["commentRichTextBox"].Font = ResourceService.DefaultMonospacedFont;
			
			// Work around WinForms/XmlForms bug:
			ControlDictionary["splitter1"].Height = 3;
		}
		
		public void ShowError(Exception ex)
		{
			TextBox txt = new TextBox();
			txt.Multiline = true;
			txt.ReadOnly = true;
			txt.BackColor = SystemColors.Window;
			SvnClientException svn;
			txt.Text = "";
			while ((svn = ex as SvnClientException) != null) {
				txt.Text += svn.SvnError + Environment.NewLine;
				ex = svn.InnerException;
			}
			if (ex != null) {
				txt.Text += ex.ToString();
			}
			txt.Dock = DockStyle.Fill;
			revisionList.Controls.Add(txt);
		}
		
		int lastRevision = -1;
		
		public void AddLogMessage(LogMessage logMessage)
		{
			if (lastRevision < 0)
				lastRevision = logMessage.Revision;
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Author,
			                                        	logMessage.Date.ToString(),
			                                        	logMessage.Message
			                                        });
			newItem.Tag = logMessage;
			revisionList.Items.Add(newItem);
		}
		
		void RevisionListViewSelectionChanged(object sender, EventArgs e)
		{
			changesList.Items.Clear();
			if (revisionList.SelectedItems.Count == 0) {
				ControlDictionary["commentRichTextBox"].Text = "";
				ControlDictionary["commentRichTextBox"].Enabled = false;
				return;
			}
			ControlDictionary["commentRichTextBox"].Enabled = true;
			ListViewItem item = revisionList.SelectedItems[0];
			LogMessage logMessage = item.Tag as LogMessage;
			ControlDictionary["commentRichTextBox"].Text = logMessage.Message;
			ChangedPathDictionary changes = logMessage.ChangedPaths;
			if (changes == null) {
				changesList.Items.Add("Loading...");
				if (!isLoadingChangedPaths) {
					isLoadingChangedPaths = true;
					loadChangedPathsItem = item;
					ThreadPool.QueueUserWorkItem(LoadChangedPaths);
				}
			} else {
				int pathWidth = 70;
				int copyFromWidth = 70;
				using (Graphics g = CreateGraphics()) {
					foreach (DictionaryEntry entry in changes) {
						string path = (string)entry.Key;
						path = path.Replace('\\', '/');
						SizeF size = g.MeasureString(path, changesList.Font);
						if (size.Width + 4 > pathWidth)
							pathWidth = (int)size.Width + 4;
						ChangedPath change = (ChangedPath)entry.Value;
						string copyFrom = change.CopyFromPath;
						if (copyFrom == null) {
							copyFrom = string.Empty;
						} else {
							copyFrom = copyFrom + " : r" + change.CopyFromRevision;
							size = g.MeasureString(copyFrom, changesList.Font);
							if (size.Width + 4 > copyFromWidth)
								copyFromWidth = (int)size.Width + 4;
						}
						ListViewItem newItem = new ListViewItem(new string[] {
						                                        	SvnClient.GetActionString(change.Action),
						                                        	path,
						                                        	copyFrom
						                                        });
						changesList.Items.Add(newItem);
					}
				}
				changesList.Columns[1].Width = pathWidth;
				changesList.Columns[2].Width = copyFromWidth;
			}
		}
		
		ListViewItem loadChangedPathsItem;
		volatile bool isLoadingChangedPaths;
		
		void LoadChangedPaths(object state)
		{
			try {
				LogMessage logMessage = (LogMessage)loadChangedPathsItem.Tag;
				string fileName = System.IO.Path.GetFullPath(viewContent.PrimaryFileName);
				Client client = SvnClient.Instance.Client;
				try {
					client.Log(new string[] { fileName },
					           Revision.FromNumber(logMessage.Revision), // Revision start
					           Revision.FromNumber(logMessage.Revision), // Revision end
					           true,                   // bool discoverChangePath
					           false,                  // bool strictNodeHistory
					           new LogMessageReceiver(ReceiveChangedPaths));
				} catch (SvnClientException ex) {
					if (ex.ErrorCode == 160013) {
						// This can happen when the file was renamed/moved so it cannot be found
						// directly in the old revision. In that case, we do a full download of
						// all revisions (so the file can be found in the new revision and svn can
						// follow back its history).
						client.Log(new string[] { fileName },
						           Revision.FromNumber(1),            // Revision start
						           Revision.FromNumber(lastRevision), // Revision end
						           true,                   // bool discoverChangePath
						           false,                  // bool strictNodeHistory
						           new LogMessageReceiver(ReceiveAllChangedPaths));
					} else {
						throw;
					}
				}
				loadChangedPathsItem = null;
				isLoadingChangedPaths = false;
				WorkbenchSingleton.SafeThreadAsyncCall<object, EventArgs>(this.RevisionListViewSelectionChanged, null, EventArgs.Empty);
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void ReceiveChangedPaths(LogMessage logMessage)
		{
			loadChangedPathsItem.Tag = logMessage;
		}
		
		void ReceiveAllChangedPaths(LogMessage logMessage)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(this.ReceiveAllChangedPathsInvoked, logMessage);
		}
		
		void ReceiveAllChangedPathsInvoked(LogMessage logMessage)
		{
			foreach (ListViewItem item in revisionList.Items) {
				LogMessage oldMessage = (LogMessage)item.Tag;
				if (oldMessage.Revision == logMessage.Revision) {
					item.Tag = logMessage;
					break;
				}
			}
		}
	}
}
