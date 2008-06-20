// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class EnvironmentNode : AbstractNode
	{
		public string[] ColorNames;
		public string[] ColorDescs;
		public EditorHighlightColor[] Colors;
		
		const string CustomColorPrefix = "Custom$";
		
		public EnvironmentNode(XmlElement el)
		{
			List<EditorHighlightColor> envColors = new List<EditorHighlightColor>();
			List<string> envColorNames = new List<string>();
			List<string> envColorDescriptions = new List<string>();
			
			if (el != null) {
				foreach (XmlNode node in el.ChildNodes) {
					if (node is XmlElement) {
						if (node.Name == "Custom") {
							envColorNames.Add(CustomColorPrefix + (node as XmlElement).GetAttribute("name"));
							envColorDescriptions.Add((node as XmlElement).GetAttribute("name"));
						} else {
							envColorNames.Add(node.Name);
							envColorDescriptions.Add("${res:Dialog.HighlightingEditor.EnvColors." + node.Name + "}");
						}
						envColors.Add(new EditorHighlightColor((XmlElement)node));
					}
				}
			}
			
			foreach (KeyValuePair<string, HighlightColor> pair in new DefaultHighlightingStrategy().EnvironmentColors) {
				if (!envColorNames.Contains(pair.Key)) {
					envColorNames.Add(pair.Key);
					envColorDescriptions.Add("${res:Dialog.HighlightingEditor.EnvColors." + pair.Key + "}");
					envColors.Add(EditorHighlightColor.FromTextEditor(pair.Value));
				}
			}
			
			this.ColorNames = envColorNames.ToArray();
			this.ColorDescs = envColorDescriptions.ToArray();
			this.Colors = envColors.ToArray();
			StringParser.Parse(ColorDescs);
			
			Text = ResNodeName("EnvironmentColors");
			
			panel = new EnvironmentOptionPanel(this);
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Environment");
			for (int i = 0; i < ColorNames.Length; i++) {
				if (ColorNames[i].StartsWith(CustomColorPrefix)) {
					writer.WriteStartElement("Custom");
					writer.WriteAttributeString("name", ColorNames[i].Substring(CustomColorPrefix.Length));
				} else {
					writer.WriteStartElement(ColorNames[i]);
				}
				Colors[i].WriteXmlAttributes(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
	
	class EnvironmentOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.ListView listView;

		public EnvironmentOptionPanel(EnvironmentNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Environment.xfrm"));
			
			button = (Button)ControlDictionary["button"];
			button.Click += new EventHandler(EditButtonClicked);
			listView  = (ListView)ControlDictionary["listView"];
			
			listView.Font = new Font(listView.Font.FontFamily, 10);
		}
		
		public override void StoreSettings()
		{
			EnvironmentNode node = (EnvironmentNode)parent;
			
			foreach (EnvironmentItem item in listView.Items) {
				node.Colors[item.arrayIndex] = item.Color;
			}
		}
		
		public override void LoadSettings()
		{
			EnvironmentNode node = (EnvironmentNode)parent;
			listView.Items.Clear();
			
			for (int i = 0; i <= node.ColorNames.GetUpperBound(0); ++i) {
				listView.Items.Add(new EnvironmentItem(i, node.ColorDescs[i], node.Colors[i], listView.Font));
			}
		}
		
		void EditButtonClicked(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			EnvironmentItem item = (EnvironmentItem)listView.SelectedItems[0];
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(item.Color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					item.Color = dlg.Color;
					item.ColorUpdate();
				}
			}
		}
		
		private class EnvironmentItem : ListViewItem
		{
			public string Name;
			public EditorHighlightColor Color;
			public int arrayIndex;
			
			Font basefont;
			Font listfont;
			
			public EnvironmentItem(int index, string name, EditorHighlightColor color, Font listFont) : base(new string[] {name, "Sample"})
			{
				Name = name;
				Color = color;
				arrayIndex = index;
				
				this.UseItemStyleForSubItems = false;
				
				basefont = SharpDevelopTextEditorProperties.Instance.FontContainer.DefaultFont;
				listfont = listFont;
				
				ColorUpdate();
			}
			
			public void ColorUpdate()
			{
				FontStyle fs = FontStyle.Regular;
				if (Color.Bold)   fs |= FontStyle.Bold;
				if (Color.Italic) fs |= FontStyle.Italic;
				
				this.Font = new Font(listfont.FontFamily, 8);
				
				Font font = new Font(basefont, fs);
				
				this.SubItems[1].Font = font;
				
				this.SubItems[1].ForeColor = Color.GetForeColor();
				this.SubItems[1].BackColor = Color.GetBackColor();
			}
		}
	}
}
