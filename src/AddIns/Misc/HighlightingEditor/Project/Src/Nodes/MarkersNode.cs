// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class MarkersNode : AbstractNode
	{
		public MarkersNode(XmlElement el, bool prev)
		{
			Text = ResNodeName(prev ? "MarkPreviousWord" : "MarkNextWord");
			
			panel = new MarkersOptionPanel(this, prev);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName(prev ? "MarkPrevious" : "MarkFollowing");
			
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new MarkerNode(el2, prev));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "";
			foreach (MarkerNode node in Nodes) {
				ret += node.ToXml();
			}
			return ret;
		}
	}
	
	class MarkersOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.ListView listView;
		
		bool previous = false;
		
		public MarkersOptionPanel(MarkersNode parent, bool prev) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Markers.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
			
			previous = prev;
			ControlDictionary["label"].Text = ResourceService.GetString(previous ? "Dialog.HighlightingEditor.Markers.Previous" : "Dialog.HighlightingEditor.Markers.Next");
		}
		
		public override void StoreSettings()
		{
		}
		
		public override void LoadSettings()
		{
			MarkersNode node = (MarkersNode)parent;
			listView.Items.Clear();
			
			foreach (MarkerNode rn in node.Nodes) {
				ListViewItem lv = new ListViewItem(rn.What);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.Markers.EnterName");
				if (box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				MarkerNode rsn = new MarkerNode(box.TextBox.Text, previous);
				ListViewItem lv = new ListViewItem(box.TextBox.Text);
				lv.Tag = rsn;
				parent.Nodes.Add(rsn);
				listView.Items.Add(lv);
			}
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			((TreeNode)listView.SelectedItems[0].Tag).Remove();
			listView.SelectedItems[0].Remove();
		}
	}
}
