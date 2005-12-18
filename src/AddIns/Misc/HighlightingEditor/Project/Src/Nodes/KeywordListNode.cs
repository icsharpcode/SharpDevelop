// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Text;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class KeywordListNode : AbstractNode
	{
		EditorHighlightColor color;
		StringCollection words = new StringCollection();
		string name;
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public StringCollection Words {
			get {
				return words;
			}
			set {
				if (words != null) {
					words.Clear();
				}
				words = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public KeywordListNode(XmlElement el)
		{
			Text = ResNodeName("KeywordList");
			panel = new KeywordListOptionPanel(this);
				
			if (el == null) return;

			color = new EditorHighlightColor(el);
			
			XmlNodeList keys = el.GetElementsByTagName("Key");
			foreach (XmlElement node in keys) {
				if (node.Attributes["word"] != null) words.Add(node.Attributes["word"].InnerText);
			}
			
			if (el.Attributes["name"] != null) {
				name = el.Attributes["name"].InnerText;
			}
			UpdateNodeText();
			
		}
		
		public KeywordListNode(string Name)
		{
			name = Name;
			color = new EditorHighlightColor();
			UpdateNodeText();

			panel = new KeywordListOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "") Text = name;
		}
		
		public override string ToXml()
		{
			StringBuilder ret = new StringBuilder("\t\t\t<KeyWords name=\"");
			ret.Append(ReplaceXmlChars(name));
			ret.Append("\" ");
			////ret += color.ToXml() + ">\n";
			ret.Append(color.ToXml());
			ret.Append(">\n");
			foreach(string str in words) {
				////ret += "\t\t\t\t<Key word=\"" + ReplaceXmlChars(str) + "\"/>\n";
				ret.Append("\t\t\t\t<Key word=\"");
				ret.Append(ReplaceXmlChars(str));
				ret.Append("\"/>\n");
			}
			////ret += "\t\t\t</KeyWords>\n\n";
			ret.Append("\t\t\t</KeyWords>\n\n");
			return ret.ToString();
		}
	}
	
	class KeywordListOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button chgBtn;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label sampleLabel;

		public KeywordListOptionPanel(KeywordListNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.KeywordList.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addBtnClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeBtnClick);
			chgBtn = (Button)ControlDictionary["chgBtn"];
			chgBtn.Click += new EventHandler(chgBtnClick);
			
			nameBox = (TextBox)ControlDictionary["nameBox"];
			sampleLabel = (Label)ControlDictionary["sampleLabel"];
			listBox  = (ListBox)ControlDictionary["listBox"];
		}
		
		EditorHighlightColor color;
		
		public override void StoreSettings()
		{
			KeywordListNode node = (KeywordListNode)parent;
			StringCollection col = new StringCollection();
			
			foreach (string word in listBox.Items) {
				col.Add(word);
			}
			node.Words = col;
			node.Name = nameBox.Text;
			node.Color = color;
		}
		
		public override void LoadSettings()
		{
			KeywordListNode node = (KeywordListNode)parent;
			listBox.Items.Clear();
			
			foreach (string word in node.Words) {
				listBox.Items.Add(word);
			}
			
			Properties properties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			sampleLabel.Font = ParseFont(properties.Get("DefaultFont", ResourceService.CourierNew10.ToString()));

			color = node.Color;
			nameBox.Text = node.Name;
			PreviewUpdate(sampleLabel, color);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage("${res:Dialog.HighlightingEditor.KeywordList.NameEmpty}");
				return false;
			}
			
			return true;
		}
		
		void chgBtnClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(sampleLabel, color);
				}
			}
		}
		
		void addBtnClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.KeywordList.EnterName");
				if (box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (string item in listBox.Items) {
					if (item == box.TextBox.Text)
						return;
				}
				
				listBox.Items.Add(box.TextBox.Text);
			}
		}
		
		void removeBtnClick(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1) return;
			
			listBox.Items.RemoveAt(listBox.SelectedIndex);
		}
	}
}
