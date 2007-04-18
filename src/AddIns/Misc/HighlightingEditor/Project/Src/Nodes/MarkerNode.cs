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
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class MarkerNode : AbstractNode
	{
		bool        previous;
		string      what;
		EditorHighlightColor color;
		bool        markMarker = false;
		
		public MarkerNode(XmlElement el, bool prev)
		{
			Text = "Marker";
			previous = prev;
			panel = new MarkerOptionPanel(this, prev);
			
			if (el == null) return;
			
			color = new EditorHighlightColor(el);
			what  = el.InnerText;
			if (el.Attributes["markmarker"] != null) {
				markMarker = Boolean.Parse(el.Attributes["markmarker"].InnerText);
			}
			
			UpdateNodeText();
			
		}
		
		public MarkerNode(string What, bool prev)
		{
			what = What;
			previous = prev;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			panel = new MarkerOptionPanel(this, prev);
		}
		
		public override void UpdateNodeText()
		{
			Text = what;
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Mark" + (previous ? "Previous" : "Following"));
			color.WriteXmlAttributes(writer);
			if (markMarker) writer.WriteAttributeString("markmarker", "true");
			writer.WriteString(what);
			writer.WriteEndElement();
		}
		
		public string What {
			get {
				return what;
			}
			set {
				what = value;
			}
		}
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public bool MarkMarker {
			get {
				return markMarker;
			}
			set {
				markMarker = value;
			}
		}
		
	}
	
	class MarkerOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button chgBtn;
		private System.Windows.Forms.CheckBox checkBox;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label sampleLabel;
		
		bool previous;
		
		public MarkerOptionPanel(MarkerNode parent, bool prev) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Marker.xfrm"));
			
			chgBtn = (Button)ControlDictionary["chgBtn"];
			chgBtn.Click += new EventHandler(chgBtnClick);
			
			checkBox  = (CheckBox)ControlDictionary["checkBox"];
			nameBox   = (TextBox)ControlDictionary["nameBox"];
			sampleLabel = (Label)ControlDictionary["sampleLabel"];

			previous = prev;
			ControlDictionary["explLabel"].Text = ResourceService.GetString(previous ? "Dialog.HighlightingEditor.Marker.ExplanationPrev" : "Dialog.HighlightingEditor.Marker.ExplanationNext");
		}
		
		EditorHighlightColor color;
		
		public override void StoreSettings()
		{
			MarkerNode node = (MarkerNode)parent;
			
			node.What = nameBox.Text;
			node.Color = color;
			node.MarkMarker = checkBox.Checked;
		}
		
		public override void LoadSettings()
		{
			MarkerNode node = (MarkerNode)parent;
			
			
			sampleLabel.Font = SharpDevelopTextEditorProperties.Instance.FontContainer.DefaultFont;

			color = node.Color;
			nameBox.Text = node.What;
			checkBox.Checked = node.MarkMarker;
			PreviewUpdate(sampleLabel, color);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Marker.NameEmpty"));
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
	}
}
