// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class SpansNode : AbstractNode
	{
		public SpansNode(XmlElement el)
		{
			Text = ResNodeName("Spans");
			
			panel = new SpansOptionPanel(this);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("Span");
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new SpanNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			foreach (SpanNode node in Nodes) {
				node.WriteXml(writer);
			}
		}
	}
	
	class SpansOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button removeBtn;
		
		public SpansOptionPanel(SpansNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Spans.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
		}
		
		public override void StoreSettings()
		{
		}
		
		public override void LoadSettings()
		{
			SpansNode node = (SpansNode)parent;
			listView.Items.Clear();
			
			foreach (SpanNode rn in node.Nodes) {
				ListViewItem lv = new ListViewItem(rn.Text);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			string result = MessageService.ShowInputBox("", "${res:Dialog.HighlightingEditor.Spans.EnterName}", "");
			if (string.IsNullOrEmpty(result))
				return;
			SpanNode rsn = new SpanNode(result);
			ListViewItem lv = new ListViewItem(rsn.Text);
			lv.Tag = rsn;
			parent.Nodes.Add(rsn);
			listView.Items.Add(lv);
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			((TreeNode)listView.SelectedItems[0].Tag).Remove();
			listView.SelectedItems[0].Remove();
		}
	}
}
