// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class PropertiesNode : AbstractNode
	{
		public Hashtable Properties = new Hashtable();
		
		public PropertiesNode(XmlElement el)
		{
			Text = ResNodeName("Properties");
			panel = new PropertiesOptionPanel(this);

			if (el == null) return;
			
			foreach (XmlElement el2 in el.ChildNodes) {
				if (el2.Attributes["name"] == null || el2.Attributes["value"] == null) continue;
				Properties.Add(el2.Attributes["name"].InnerText, el2.Attributes["value"].InnerText);
			}
			
		}
	
		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "\t<Properties>\n";
			foreach (DictionaryEntry de in Properties) {
				ret += "\t\t<Property name=\"" + ReplaceXmlChars((string)de.Key) 
						+ "\" value=\"" + ReplaceXmlChars((string)de.Value) + "\"/>\n";
			}
			ret += "\t</Properties>\n\n";
			return ret;
		}
	}
	
	class PropertiesOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button editBtn;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.ListView listView;	
		
		public PropertiesOptionPanel(PropertiesNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Properties.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			editBtn = (Button)ControlDictionary["editBtn"];
			editBtn.Click += new EventHandler(editClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
		}
	
		public override void StoreSettings()
		{
			PropertiesNode node = (PropertiesNode)parent;
			
			node.Properties.Clear();
			foreach (ListViewItem item in listView.Items) {
				node.Properties.Add(item.Text, item.SubItems[1].Text);
			}
		}
		
		public override void LoadSettings()
		{
			PropertiesNode node = (PropertiesNode)parent;
			listView.Items.Clear();
			
			foreach (DictionaryEntry de in node.Properties) {
				ListViewItem lv = new ListViewItem(new string[] {(string)de.Key, (string)de.Value});
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.Properties.EnterName");
				if (box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.Cancel) return;
				
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				listView.Items.Add(new ListViewItem(new string[] {box.TextBox.Text, ""}));
			}
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			listView.SelectedItems[0].Remove();
		}
		
		void editClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;

			using (InputBox box = new InputBox()) {
				box.Text = ResourceService.GetString("Dialog.HighlightingEditor.EnterText");
				box.Label.Text = String.Format(ResourceService.GetString("Dialog.HighlightingEditor.Properties.EnterValue"), listView.SelectedItems[0].Text);
				if (box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.Cancel) return;
				
				listView.SelectedItems[0].SubItems[1].Text = box.TextBox.Text;
			}
		}
	}
}
