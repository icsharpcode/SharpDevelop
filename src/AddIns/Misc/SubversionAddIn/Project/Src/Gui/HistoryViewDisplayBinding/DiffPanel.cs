// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of InfoPanel.
	/// </summary>
	public class DiffPanel : BaseSharpDevelopUserControl
	{
		IViewContent viewContent;
		
		public DiffPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.DiffPanel.xfrm"));
			Get<ListView>("fromRevision").SelectedIndexChanged += new EventHandler(ShowDiff);
			Get<ListView>("toRevision").SelectedIndexChanged += new EventHandler(ShowDiff);
			
			ControlDictionary["diffRichTextBox"].Enabled = false;
			ControlDictionary["diffRichTextBox"].Font = new Font("Courier New", 10);
			ControlDictionary["splitter1"].Height = 3;
			
			ListViewItem newItem;
			newItem = new ListViewItem(new string[] { "Base", "", "", "" });
			newItem.Tag = Revision.Base;
			Get<ListView>("fromRevision").Items.Add(newItem);
			newItem.Selected = true;
			newItem = new ListViewItem(new string[] { "Work", "", "", "" });
			newItem.Tag = Revision.Working;
			Get<ListView>("toRevision").Items.Add(newItem);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			// fix sizing problems
			Get<ListView>("toRevision").Width -= 13;
			ControlDictionary["fromRevisionPanel"].Width = ControlDictionary["topPanel"].Width / 2 - 2;
		}
		
		string output = null;
		string fileName = null;
		Revision fromRevision;
		Revision toRevision;
		
		void DoDiffOperation()
		{
			output = null;
			MemoryStream outStream = new MemoryStream();
			MemoryStream errStream = new MemoryStream();
			SvnClient.Instance.Client.Diff(new string [] {} ,
			                               fileName,
			                               fromRevision,
			                               fileName,
			                               toRevision,
			                               false,
			                               false,
			                               true,
			                               outStream,
			                               errStream);
			output = Encoding.Default.GetString(outStream.ToArray());
			WorkbenchSingleton.SafeThreadCall(this, "SetOutput");
		}
		
		void SetOutput()
		{
			ControlDictionary["diffRichTextBox"].Enabled = true;
			ControlDictionary["diffLabel"].Text = "Diff from revision " + fromRevision + " to " + toRevision + ":";
			ControlDictionary["diffRichTextBox"].Text = output;
		}
		
		void Disable()
		{
			ControlDictionary["diffRichTextBox"].Enabled = false;
			ControlDictionary["diffLabel"].Text = "Diff:";
			ControlDictionary["diffRichTextBox"].Text = "";
		}
		
		void ShowDiff(object sender, EventArgs e)
		{
			Disable();
			
			if (Get<ListView>("fromRevision").SelectedItems.Count == 0 || Get<ListView>("toRevision").SelectedItems.Count == 0 ) {
				return;
			}
			
			fromRevision = Get<ListView>("fromRevision").SelectedItems[0].Tag as Revision;
			toRevision   = Get<ListView>("toRevision").SelectedItems[0].Tag as Revision;
			fileName     = Path.GetFullPath(viewContent.FileName);
			
			if (fromRevision.ToString() == toRevision.ToString()) {
				output = "";
			} else {
				SvnClient.Instance.OperationStart("Diff", new ThreadStart(DoDiffOperation));
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
			Get<ListView>("fromRevision").Items.Add(newItem);
			
			ListViewItem newItem2 = new ListViewItem(new string[] {
			                                         	logMessage.Revision.ToString(),
			                                         	logMessage.Author,
			                                         	logMessage.Date.ToString(),
			                                         	logMessage.Message
			                                         });
			newItem2.Tag = Revision.FromNumber(logMessage.Revision);
			Get<ListView>("toRevision").Items.Add(newItem2);
		}
	}
}
