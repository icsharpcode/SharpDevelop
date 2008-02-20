// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using System.IO;
using System.Text;

namespace ICSharpCode.Svn
{
	public partial class DiffPanel
	{
		IViewContent viewContent;
		TextEditorControl textEditor;
		
		public DiffPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			textEditor = new TextEditorControl();
			textEditor.Dock = DockStyle.Fill;
			diffViewPanel.Controls.Add(textEditor);
			
			textEditor.TextEditorProperties = SharpDevelopTextEditorProperties.Instance;
			textEditor.Document.ReadOnly = true;
			textEditor.Enabled = false;
			
			textEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("Patch");
			
			ListViewItem newItem;
			newItem = new ListViewItem(new string[] { "Base", "", "", "" });
			newItem.Tag = Revision.Base;
			leftListView.Items.Add(newItem);
			newItem.Selected = true;
			newItem = new ListViewItem(new string[] { "Work", "", "", "" });
			newItem.Tag = Revision.Working;
			rightListView.Items.Add(newItem);
		}
		
		void LeftListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			ShowDiff();
		}
		
		void RightListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			ShowDiff();
		}
		
		/*
		protected override void OnLoad(EventArgs e)
		{
			// fix sizing problems
			Get<ListView>("toRevision").Width -= 13;
			ControlDictionary["fromRevisionPanel"].Width = ControlDictionary["topPanel"].Width / 2 - 2;
		}
		 */
		
		string output = null;
		string fileName = null;
		Revision fromRevision;
		Revision toRevision;
		
		void DoDiffOperation()
		{
			output = null;
			MemoryStream outStream = new MemoryStream();
			MemoryStream errStream = new MemoryStream();
//			SvnClient.Instance.Client.Diff(new string [] {} ,
//			                               fileName,
//			                               fromRevision,
//			                               fileName,
//			                               toRevision,
//			                               Recurse.None,
//			                               false,
//			                               true,
//			                               outStream,
//			                               errStream);
			output = Encoding.Default.GetString(outStream.ToArray());
			WorkbenchSingleton.SafeThreadCall(SetOutput);
		}
		
		void SetOutput()
		{
			textEditor.Enabled = true;
			diffLabel.Text = "Diff from revision " + fromRevision + " to " + toRevision + ":";
			textEditor.Text = output;
		}
		
		void Disable()
		{
			textEditor.Enabled = false;
			diffLabel.Text = "Diff:";
			textEditor.Text = "";
		}
		
		void ShowDiff()
		{
			Disable();
			
			if (leftListView.SelectedItems.Count == 0 || rightListView.SelectedItems.Count == 0 ) {
				return;
			}
			
			fromRevision = leftListView.SelectedItems[0].Tag as Revision;
			toRevision   = rightListView.SelectedItems[0].Tag as Revision;
			fileName     = Path.GetFullPath(viewContent.PrimaryFileName);
			
			if (fromRevision.ToString() == toRevision.ToString()) {
				output = "";
			} else {
				//SvnClient.Instance.OperationStart("Diff", DoDiffOperation);
			}
		}

		public void AddLogMessage(LogMessage logMessage)
		{
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Author,
			                                        	logMessage.Date.ToString(),
			                                        	logMessage.Message
			                                        });
			newItem.Tag = Revision.FromNumber(logMessage.Revision);
			leftListView.Items.Add(newItem);
			
			ListViewItem newItem2 = new ListViewItem(new string[] {
			                                         	logMessage.Revision.ToString(),
			                                         	logMessage.Author,
			                                         	logMessage.Date.ToString(),
			                                         	logMessage.Message
			                                         });
			newItem2.Tag = Revision.FromNumber(logMessage.Revision);
			rightListView.Items.Add(newItem2);
		}
	}
}
