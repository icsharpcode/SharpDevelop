// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Properties");
			foreach (DictionaryEntry de in Properties) {
				writer.WriteStartElement("Property");
				writer.WriteAttributeString("name", (string)de.Key);
				writer.WriteAttributeString("value", (string)de.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
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
			string result = MessageService.ShowInputBox("", "${res:Dialog.HighlightingEditor.Properties.EnterName}", "");
			if (string.IsNullOrEmpty(result))
				return;
			
			foreach (ListViewItem item in listView.Items) {
				if (item.Text == result)
					return;
			}
			
			listView.Items.Add(new ListViewItem(new string[] {result, ""}));
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			listView.SelectedItems[0].Remove();
		}
		
		void editClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;

			string result = MessageService.ShowInputBox("${res:Dialog.HighlightingEditor.EnterText}",
			                                            String.Format(ResourceService.GetString("Dialog.HighlightingEditor.Properties.EnterValue"), listView.SelectedItems[0].Text),
			                                            "");
			if (string.IsNullOrEmpty(result))
				return;
			
			listView.SelectedItems[0].SubItems[1].Text = result;
		}
	}
}
