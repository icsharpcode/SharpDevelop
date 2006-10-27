// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class TaskListOptions : AbstractOptionPanel
	{
		const string taskListView        = "taskListView";
		const string nameTextBox         = "nameTextBox";
		const string changeButton        = "changeButton";
		const string removeButton        = "removeButton";
		const string addButton           = "addButton";
		ListView taskList;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.TaskListOptions.xfrm"));
			
			string[] tokens = PropertyService.Get("SharpDevelop.TaskListTokens", ParserService.DefaultTaskListTokens);
			taskList = (ListView)ControlDictionary[taskListView];
			taskList.BeginUpdate();
			foreach (string token in tokens) {
				taskList.Items.Add(token);
			}
			taskList.EndUpdate();
			taskList.SelectedIndexChanged += new EventHandler(TaskListViewSelectedIndexChanged);
			
			ControlDictionary[changeButton].Click += new EventHandler(ChangeButtonClick);
			ControlDictionary[removeButton].Click += new EventHandler(RemoveButtonClick);
			ControlDictionary[addButton].Click    += new EventHandler(AddButtonClick);
			
			TaskListViewSelectedIndexChanged(this, EventArgs.Empty);
		}
		
		public override bool StorePanelContents()
		{
			List<string> tokens = new List<string>();
			
			foreach (ListViewItem item in taskList.Items) {
				string text = item.Text.Trim();
				if (text.Length > 0) {
					tokens.Add(text);
				}
			}
			
			PropertyService.Set("SharpDevelop.TaskListTokens", tokens.ToArray());
			
			return true;
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			string newItemText = ControlDictionary[nameTextBox].Text;
			foreach (ListViewItem item in ((ListView)ControlDictionary[taskListView]).Items) {
				if (item.Text == newItemText) {
					return;
				}
			}
			((ListView)ControlDictionary[taskListView]).Items.Add(new ListViewItem(newItemText));
		}
		
		void ChangeButtonClick(object sender, EventArgs e)
		{
			((ListView)ControlDictionary[taskListView]).SelectedItems[0].Text = ControlDictionary[nameTextBox].Text;
		}
		void RemoveButtonClick(object sender, EventArgs e)
		{
			((ListView)ControlDictionary[taskListView]).Items.Remove(((ListView)ControlDictionary[taskListView]).SelectedItems[0]);
		}
		
		void TaskListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary[taskListView]).SelectedItems.Count > 0) {
				ControlDictionary[nameTextBox].Text = ((ListView)ControlDictionary[taskListView]).SelectedItems[0].Text;
				ControlDictionary[changeButton].Enabled = true;
				ControlDictionary[removeButton].Enabled = true;
			} else {
				ControlDictionary[nameTextBox].Text = String.Empty;
				ControlDictionary[changeButton].Enabled = false;
				ControlDictionary[removeButton].Enabled = false;
			}
		}
		
		public TaskListOptions()
		{
		}
	}
}
