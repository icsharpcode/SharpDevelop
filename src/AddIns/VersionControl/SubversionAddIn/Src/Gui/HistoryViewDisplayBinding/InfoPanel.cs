// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of InfoPanel.
	/// </summary>
	public partial class InfoPanel : UserControl
	{
		string fileName;
		
		public InfoPanel(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.fileName = fileName;
			
			InitializeComponent();
			
			revisionListView.SelectedIndexChanged += RevisionListViewSelectionChanged;
			commentRichTextBox.Font = WinFormsResourceService.DefaultMonospacedFont;
			commentRichTextBox.Enabled = false;
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
				txt.Text += svn.Message + Environment.NewLine;
				ex = svn.GetInnerException();
			}
			if (ex != null) {
				txt.Text += ex.ToString();
			}
			txt.Dock = DockStyle.Fill;
			revisionListView.Controls.Add(txt);
		}
		
		long lastRevision = -1;
		
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
			revisionListView.Items.Add(newItem);
		}
		
		void RevisionListViewSelectionChanged(object sender, EventArgs e)
		{
			changesListView.Items.Clear();
			if (revisionListView.SelectedItems.Count == 0) {
				commentRichTextBox.Text = "";
				commentRichTextBox.Enabled = false;
				return;
			}
			commentRichTextBox.Enabled = true;
			ListViewItem item = revisionListView.SelectedItems[0];
			LogMessage logMessage = item.Tag as LogMessage;
			commentRichTextBox.Text = logMessage.Message;
			List<ChangedPath> changes = logMessage.ChangedPaths;
			if (changes == null) {
				changesListView.Items.Add("Loading...");
				if (!isLoadingChangedPaths) {
					isLoadingChangedPaths = true;
					loadChangedPathsItem = item;
					ThreadPool.QueueUserWorkItem(LoadChangedPaths);
				}
			} else {
				int pathWidth = 70;
				int copyFromWidth = 70;
				using (Graphics g = CreateGraphics()) {
					foreach (ChangedPath change in changes) {
						string path = change.Path;
						path = path.Replace('\\', '/');
						SizeF size = g.MeasureString(path, changesListView.Font);
						if (size.Width + 4 > pathWidth)
							pathWidth = (int)size.Width + 4;
						string copyFrom = change.CopyFromPath;
						if (copyFrom == null) {
							copyFrom = string.Empty;
						} else {
							copyFrom = copyFrom + " : r" + change.CopyFromRevision;
							size = g.MeasureString(copyFrom, changesListView.Font);
							if (size.Width + 4 > copyFromWidth)
								copyFromWidth = (int)size.Width + 4;
						}
						ListViewItem newItem = new ListViewItem(new string[] {
						                                        	SvnClientWrapper.GetActionString(change.Action),
						                                        	path,
						                                        	copyFrom
						                                        });
						changesListView.Items.Add(newItem);
					}
				}
				changesListView.Columns[1].Width = pathWidth;
				changesListView.Columns[2].Width = copyFromWidth;
			}
		}
		
		ListViewItem loadChangedPathsItem;
		volatile bool isLoadingChangedPaths;
		
		void LoadChangedPaths(object state)
		{
			try {
				LogMessage logMessage = (LogMessage)loadChangedPathsItem.Tag;
				using (SvnClientWrapper client = new SvnClientWrapper()) {
					client.AllowInteractiveAuthorization();
					try {
						client.Log(new string[] { fileName },
						           Revision.FromNumber(logMessage.Revision), // Revision start
						           Revision.FromNumber(logMessage.Revision), // Revision end
						           int.MaxValue,           // limit
						           true,                   // bool discoverChangePath
						           false,                  // bool strictNodeHistory
						           ReceiveChangedPaths);
					} catch (SvnClientException ex) {
						if (ex.IsKnownError(KnownError.FileNotFound)) {
							// This can happen when the file was renamed/moved so it cannot be found
							// directly in the old revision. In that case, we do a full download of
							// all revisions (so the file can be found in the new revision and svn can
							// follow back its history).
							client.Log(new string[] { fileName },
							           Revision.FromNumber(1),            // Revision start
							           Revision.FromNumber(lastRevision), // Revision end
							           int.MaxValue,           // limit
							           true,                   // bool discoverChangePath
							           false,                  // bool strictNodeHistory
							           ReceiveAllChangedPaths);
						} else {
							throw;
						}
					}
				}
				loadChangedPathsItem = null;
				isLoadingChangedPaths = false;
				SD.MainThread.InvokeAsyncAndForget(() => this.RevisionListViewSelectionChanged(null, EventArgs.Empty));
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void ReceiveChangedPaths(LogMessage logMessage)
		{
			loadChangedPathsItem.Tag = logMessage;
		}
		
		void ReceiveAllChangedPaths(LogMessage logMessage)
		{
			SD.MainThread.InvokeAsyncAndForget(() => this.ReceiveAllChangedPathsInvoked(logMessage));
		}
		
		void ReceiveAllChangedPathsInvoked(LogMessage logMessage)
		{
			foreach (ListViewItem item in revisionListView.Items) {
				LogMessage oldMessage = (LogMessage)item.Tag;
				if (oldMessage.Revision == logMessage.Revision) {
					item.Tag = logMessage;
					break;
				}
			}
		}
	}
}
