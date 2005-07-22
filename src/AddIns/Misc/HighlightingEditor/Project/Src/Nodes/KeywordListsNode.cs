// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	class KeywordListsNode : AbstractNode
	{
		public KeywordListsNode(XmlElement el)
		{
			Text = ResNodeName("KeywordLists");
			panel = new KeywordListsOptionPanel(this);
			
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("KeyWords");
			if (nodes == null) return;
			
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new KeywordListNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "";
			foreach (KeywordListNode node in Nodes) {
				ret += node.ToXml();
			}
			return ret;
		}
	}
	
	class KeywordListsOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.ListView listView;
		
		public KeywordListsOptionPanel(KeywordListsNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.KeywordLists.xfrm"));
			
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
			KeywordListsNode node = (KeywordListsNode)parent;
			listView.Items.Clear();
			
			foreach (KeywordListNode rn in node.Nodes) {
				ListViewItem lv = new ListViewItem(rn.Name);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.KeywordLists.EnterName");
				if (box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				KeywordListNode kwn = new KeywordListNode(box.TextBox.Text);
				ListViewItem lv = new ListViewItem(box.TextBox.Text);
				lv.Tag = kwn;
				parent.Nodes.Add(kwn);
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
