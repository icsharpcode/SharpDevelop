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
using System.Reflection;
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
	public class InfoPanel : BaseSharpDevelopUserControl
	{
		IViewContent viewContent;
		
		public InfoPanel(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("InfoPanel.xfrm"));
			
			((ListView)ControlDictionary["revisionListView"]).SelectedIndexChanged  += new EventHandler(RevisionListViewSelectionChanged);
			ControlDictionary["commentRichTextBox"].Enabled = false;
			ControlDictionary["commentRichTextBox"].Font = new Font("Courier New", 10);
			
			ControlDictionary["splitter1"].Height = 3;
		}
		
		void RevisionListViewSelectionChanged(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["revisionListView"]).SelectedItems.Count == 0) {
				ControlDictionary["commentRichTextBox"].Text = "";
				ControlDictionary["commentRichTextBox"].Enabled = false;
				ControlDictionary["commentLabel"].Text = "Comment:";
				return;
			}
			ControlDictionary["commentRichTextBox"].Enabled = true;
			LogMessage logMessage = ((ListView)ControlDictionary["revisionListView"]).SelectedItems[0].Tag as LogMessage;
			ControlDictionary["commentLabel"].Text = "Comment of revision " + logMessage.Revision + ":";
			ControlDictionary["commentRichTextBox"].Text = logMessage.Message;
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
