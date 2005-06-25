/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 22.11.2004
 * Time: 11:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of InfoPanel.
	/// </summary>
	public class ContentPanel : BaseSharpDevelopUserControl
	{
		IViewContent viewContent;
			
		public ContentPanel(IViewContent viewContent)
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.Svn.Resources.ContentPanel.xfrm"));
			this.viewContent = viewContent;
			((ListView)ControlDictionary["revisionListView"]).SelectedIndexChanged  += new EventHandler(RevisionListViewSelectionChanged);
			ControlDictionary["contentRichTextBox"].Enabled = false;
			ControlDictionary["contentRichTextBox"].Font = new Font("Courier New", 10);
			ControlDictionary["splitter1"].Height = 3;
		}
		
		LogMessage logMessage;
		
		void DoCat()
		{
			MemoryStream memStream = new MemoryStream();
			try {
				SvnClient.Instance.Client.Cat(memStream,
				           Path.GetFullPath(viewContent.FileName), 
				           logMessage.Revision == 1 ? Revision.Head : Revision.FromNumber(logMessage.Revision));
				
				ControlDictionary["contentRichTextBox"].Enabled = true;
				ControlDictionary["contentRichTextBox"].Text = Encoding.Default.GetString(memStream.ToArray());
				ControlDictionary["contentLabel"].Text       = "Content of revision " + logMessage.Revision + ":";
			} finally {
				memStream.Close();
			}
		}
		
		void RevisionListViewSelectionChanged(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["revisionListView"]).SelectedItems.Count == 0) {
				ControlDictionary["contentRichTextBox"].Text = "";
				ControlDictionary["contentRichTextBox"].Enabled = false;
				ControlDictionary["contentLabel"].Text = "Content:";
				return;
			}
			logMessage = ((ListView)ControlDictionary["revisionListView"]).SelectedItems[0].Tag as LogMessage;
			SvnClient.Instance.OperationStart("Cat", new ThreadStart(DoCat));
		}

		public void AddLogMessage(LogMessage logMessage)
		{
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Date.ToString(),
			                                        	logMessage.Author,
			                                        	logMessage.Message
			                                        });
			newItem.Tag = logMessage;
			((ListView)ControlDictionary["revisionListView"]).Items.Add(newItem);
		}
	}
}
