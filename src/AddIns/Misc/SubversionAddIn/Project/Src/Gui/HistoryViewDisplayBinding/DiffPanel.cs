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
			((ListView)ControlDictionary["fromRevisionListView"]).SelectedIndexChanged += new EventHandler(ShowDiff);
			((ListView)ControlDictionary["toRevisionListView"]).SelectedIndexChanged += new EventHandler(ShowDiff);
			
			ControlDictionary["diffRichTextBox"].Enabled = false;
			ControlDictionary["diffRichTextBox"].Font = new Font("Courier New", 10);
			ControlDictionary["splitter1"].Height = 3;
			
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	"Head",
			                                        	""
			                                        });
			((ListView)ControlDictionary["fromRevisionListView"]).Items.Add(newItem);
			ListViewItem newItem2 = new ListViewItem(new string[] {
			                                        	"Head",
			                                        	""
			                                        });
			((ListView)ControlDictionary["toRevisionListView"]).Items.Add(newItem2);
		}
		
		string output = null;
		string fileName = null;
		LogMessage fromLogMessage;
		LogMessage toLogMessage;
		
		Revision GetRevision(LogMessage msg)
		{
			if (fromLogMessage == null) {
				return Revision.Head;
			}
			 return msg.Revision == 1 ? Revision.Base : Revision.FromNumber(msg.Revision);
		}
		
		void DoDiffOperation()
		{
			output = null;
			MemoryStream outStream = new MemoryStream();
			MemoryStream errStream = new MemoryStream();
			SvnClient.Instance.Client.Diff(new string [] {} ,
				            fileName,
				            GetRevision(fromLogMessage),
				            fileName,
				            GetRevision(toLogMessage),
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
			ControlDictionary["diffLabel"].Text = "Diff from revision " + (fromLogMessage == null ? "Working" : fromLogMessage.Revision.ToString()) + " to " + (toLogMessage == null ? "Working" : toLogMessage.Revision.ToString()) + ":";
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
			
			if (((ListView)ControlDictionary["fromRevisionListView"]).SelectedItems.Count == 0 || ((ListView)ControlDictionary["toRevisionListView"]).SelectedItems.Count == 0 ) {
				return;
			}
			
			fromLogMessage = ((ListView)ControlDictionary["fromRevisionListView"]).SelectedItems[0].Tag as LogMessage;
			toLogMessage   = ((ListView)ControlDictionary["toRevisionListView"]).SelectedItems[0].Tag as LogMessage;
			fileName       = Path.GetFullPath(viewContent.FileName);
			
			SvnClient.Instance.OperationStart("Diff", new ThreadStart(DoDiffOperation));
		}

		public void AddLogMessage(LogMessage logMessage)
		{
			ListViewItem newItem = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Date.ToString()
			                                        });
			newItem.Tag = logMessage;
			((ListView)ControlDictionary["fromRevisionListView"]).Items.Add(newItem);
			
			ListViewItem newItem2 = new ListViewItem(new string[] {
			                                        	logMessage.Revision.ToString(),
			                                        	logMessage.Date.ToString()
			                                        });
			newItem2.Tag = logMessage;
			((ListView)ControlDictionary["toRevisionListView"]).Items.Add(newItem2);
		}
	}
}
