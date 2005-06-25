/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 22.11.2004
 * Time: 11:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
			newItem = new ListViewItem(new string[] { "Base", "" });
			newItem.Tag = Revision.Base;
			Get<ListView>("fromRevision").Items.Add(newItem);
			newItem = new ListViewItem(new string[] { "Base", "" });
			newItem.Tag = Revision.Base;
			Get<ListView>("toRevision").Items.Add(newItem);
			newItem = new ListViewItem(new string[] { "Head", "" });
			newItem.Tag = Revision.Head;
			Get<ListView>("toRevision").Items.Add(newItem);
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
			
			SvnClient.Instance.OperationStart("Diff", new ThreadStart(DoDiffOperation));
		}

		public void AddLogMessage(LogMessage logMessage)
		{
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Date.ToString()
			                                        });
			newItem.Tag = logMessage;
			Get<ListView>("fromRevision").Items.Add(newItem);
			
			ListViewItem newItem2 = new ListViewItem(new string[] {
			                                         	logMessage.Revision.ToString(),
			                                         	logMessage.Date.ToString()
			                                         });
			newItem2.Tag = logMessage;
			Get<ListView>("toRevision").Items.Add(newItem2);
		}
	}
}
