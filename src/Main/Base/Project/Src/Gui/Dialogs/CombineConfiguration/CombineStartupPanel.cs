//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//using System;
//using System.Collections;
//using System.ComponentModel;
//using System.Drawing;
//using System.Windows.Forms;
//using ICSharpCode.Core;
//
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Project;
//
//namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
//{
//	public class CombineStartupPanel : AbstractOptionPanel
//	{
//		static 
//		static 
//		
//		Solution combine;
//		
//		public override bool ReceiveDialogMessage(DialogMessage message)
//		{
//			if (message == DialogMessage.OK) {
//				// write back singlestartup project
//				combine.SingleStartProjectName = ((ComboBox)ControlDictionary["singleComboBox"]).Text;
//				combine.SingleStartupProject   = ((RadioButton)ControlDictionary["singleRadioButton"]).Checked;
//				
//				// write back new combine execute definitions
//				combine.CombineExecuteDefinitions.Clear();
//				foreach (ListViewItem item in ((ListView)ControlDictionary["entryListView"]).Items) {
//					EntryExecuteType type = EntryExecuteType.None;
//					if (item.SubItems[1].Text == ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute")) {
//						type = EntryExecuteType.Execute;
//					}
//					combine.CombineExecuteDefinitions.Add(new CombineExecuteDefinition(
//						combine.GetEntry(item.Text),
//						type
//					));
//				}
//			}
//			return true;
//		}
//		
//		void SetValues(object sender, EventArgs e)
//		{
//			this.combine = (Combine)((Properties)CustomizationObject).Get("Combine");
//			
//			((RadioButton)ControlDictionary["singleRadioButton"]).Checked =  combine.SingleStartupProject;
//			((RadioButton)ControlDictionary["multipleRadioButton"]).Checked = !combine.SingleStartupProject;
//			
//			foreach (CombineEntry entry in combine.Entries)  {
//				((ComboBox)ControlDictionary["singleComboBox"]).Items.Add(entry.Name);
//			}
//			
//			((ComboBox)ControlDictionary["singleComboBox"]).SelectedIndex = combine.GetEntryNumber(combine.SingleStartProjectName);
//			
//			((RadioButton)ControlDictionary["singleRadioButton"]).CheckedChanged += new EventHandler(CheckedChanged);
//			
//			((ListView)ControlDictionary["entryListView"]).SelectedIndexChanged += new EventHandler(SelectedEntryChanged);
//			((ComboBox)ControlDictionary["actionComboBox"]).SelectedIndexChanged += new EventHandler(OptionsChanged);
//
//			ListViewItem item;
//			CombineExecuteDefinition edef;
//			for (int n = 0; n < combine.CombineExecuteDefinitions.Count; n++) {
//				edef = (CombineExecuteDefinition)combine.CombineExecuteDefinitions[n];
//				item = new ListViewItem(new string[] {
//					edef.Entry.Name,
//					edef.Type == EntryExecuteType.None ? ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None") : ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute")
//				});
//				item.Tag = edef;
//				((ListView)ControlDictionary["entryListView"]).Items.Add(item);
//			}
//			((Button)ControlDictionary["moveUpButton"]).Click += new EventHandler(OnClickMoveUpButton);
//			((Button)ControlDictionary["moveDownButton"]).Click += new EventHandler(OnClickMoveDownButtn);
//			CheckedChanged(null, null);
//		}
//
//		protected void OnClickMoveUpButton(object sender, EventArgs e)
//		{
//			ListView.SelectedIndexCollection  indexs = ((ListView)ControlDictionary["entryListView"]).SelectedIndices;
//			if (indexs.Count == 0) {
//				return;
//			}
//			int index = indexs[0];
//			if (index == 0) {
//				return;
//			}
//
//			((ListView)ControlDictionary["entryListView"]).BeginUpdate();
//			ListViewItem item = ((ListView)ControlDictionary["entryListView"]).Items[index - 1];
//			((ListView)ControlDictionary["entryListView"]).Items.Remove(item);
//			((ListView)ControlDictionary["entryListView"]).Items.Insert(index, item);
//			((ListView)ControlDictionary["entryListView"]).EndUpdate();
//
//			combine.CombineExecuteDefinitions.Remove(item.Tag);
//			combine.CombineExecuteDefinitions.Insert(index, item.Tag);
//		}
//
//		protected void OnClickMoveDownButtn(object sender, EventArgs e)
//		{
//			ListView.SelectedIndexCollection  indexs = ((ListView)ControlDictionary["entryListView"]).SelectedIndices;
//			if (indexs.Count == 0) {
//				return;
//			}
//			int index = indexs[0];
//			if (index >= (((ListView)ControlDictionary["entryListView"]).Items.Count - 1)) {
//				return;
//			}
//			((ListView)ControlDictionary["entryListView"]).BeginUpdate();
//			ListViewItem item = ((ListView)ControlDictionary["entryListView"]).Items[index + 1];
//			((ListView)ControlDictionary["entryListView"]).Items.Remove(item);
//			((ListView)ControlDictionary["entryListView"]).Items.Insert(index, item);
//			((ListView)ControlDictionary["entryListView"]).EndUpdate();
//
//			combine.CombineExecuteDefinitions.Remove(item.Tag);
//			combine.CombineExecuteDefinitions.Insert(index, item.Tag);
//		}
//		
//		public CombineStartupPanel()
//		{
//			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.CombineStartupPanel.xfrm"));
//			
//			CustomizationObjectChanged += new EventHandler(SetValues);
//		}
//		
//		void CheckedChanged(object sender, EventArgs e)
//		{
//			((Button)ControlDictionary["moveUpButton"]).Enabled = ((RadioButton)ControlDictionary["multipleRadioButton"]).Checked;
//			((Button)ControlDictionary["moveDownButton"]).Enabled = ((RadioButton)ControlDictionary["multipleRadioButton"]).Checked;
//			((ListView)ControlDictionary["entryListView"]).Enabled = ((RadioButton)ControlDictionary["multipleRadioButton"]).Checked;
//			((ComboBox)ControlDictionary["actionComboBox"]).Enabled = ((RadioButton)ControlDictionary["multipleRadioButton"]).Checked;
//
//			((ComboBox)ControlDictionary["singleComboBox"]).Enabled = ((RadioButton)ControlDictionary["singleRadioButton"]).Checked;
//		}
//		
//		void OptionsChanged(object sender, EventArgs e)
//		{
//			if (((ListView)ControlDictionary["entryListView"]).SelectedItems == null || 
//				((ListView)ControlDictionary["entryListView"]).SelectedItems.Count == 0) 
//				return;
//			ListViewItem item = ((ListView)ControlDictionary["entryListView"]).SelectedItems[0]; 
//			item.SubItems[1].Text = ((ComboBox)ControlDictionary["actionComboBox"]).SelectedItem.ToString();
//
//			int index = ((ListView)ControlDictionary["entryListView"]).SelectedIndices[0];
//			CombineExecuteDefinition edef = (CombineExecuteDefinition)combine.CombineExecuteDefinitions[index];
//
//			switch (((ComboBox)ControlDictionary["actionComboBox"]).SelectedIndex) {
//			case 0:
//				edef.Type = EntryExecuteType.None;
//				break;
//			case 1:
//				edef.Type = EntryExecuteType.Execute;
//				break;
//			default:
//				break;
//			}
//		}
//
//		void SelectedEntryChanged(object sender, EventArgs e)
//		{
//			if (((ListView)ControlDictionary["entryListView"]).SelectedItems == null ||
//				((ListView)ControlDictionary["entryListView"]).SelectedItems.Count == 0)
//				return;
//			ListViewItem item = ((ListView)ControlDictionary["entryListView"]).SelectedItems[0]; 
//			string       txt = item.SubItems[1].Text;
//			((ComboBox)ControlDictionary["actionComboBox"]).Items.Clear();
//			((ComboBox)ControlDictionary["actionComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None"));
//			((ComboBox)ControlDictionary["actionComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute"));
//			
//			if (txt == ResourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None")) {
//				((ComboBox)ControlDictionary["actionComboBox"]).SelectedIndex = 0;
//			} else {
//				((ComboBox)ControlDictionary["actionComboBox"]).SelectedIndex = 1;
//			}
//		}
//	}
//}
//
