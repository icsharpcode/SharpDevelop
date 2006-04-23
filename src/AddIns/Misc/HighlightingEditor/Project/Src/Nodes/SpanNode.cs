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
	class SpanNode : AbstractNode
	{
		bool        stopEOL;
		EditorHighlightColor color;
		EditorHighlightColor beginColor = null;
		EditorHighlightColor endColor = null;
		string      begin = "";
		string      end   = "";
		string      name  = "";
		string      rule  = "";
		bool        noEscapeSequences = false;
		
		public SpanNode(XmlElement el)
		{
			Text = ResNodeName("Span");
			
			panel = new SpanOptionPanel(this);

			if (el == null) return;
			
			color   = new EditorHighlightColor(el);
			
			if (el.Attributes["rule"] != null) {
				rule = el.Attributes["rule"].InnerText;
			}
			
			if (el.Attributes["noescapesequences"] != null) {
				noEscapeSequences = Boolean.Parse(el.Attributes["noescapesequences"].InnerText);
			}
			
			name    = el.Attributes["name"].InnerText;
			stopEOL = Boolean.Parse(el.Attributes["stopateol"].InnerText);
			begin   = el["Begin"].InnerText;
			beginColor = new EditorHighlightColor(el["Begin"]);
			
			if (el["End"] != null) {
				end  = el["End"].InnerText;
				endColor = new EditorHighlightColor(el["End"]);
			}
			
			UpdateNodeText();
			
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Span");
			writer.WriteAttributeString("name", name);
			if (noEscapeSequences)
				writer.WriteAttributeString("noescapesequences", "true");
			if (rule != "")
				writer.WriteAttributeString("rule", rule);
			writer.WriteAttributeString("stopateol", stopEOL.ToString().ToLowerInvariant());
			color.WriteXmlAttributes(writer);
			
			writer.WriteStartElement("Begin");
			if (beginColor != null && !beginColor.NoColor)
				beginColor.WriteXmlAttributes(writer);
			writer.WriteString(begin);
			writer.WriteEndElement();
			
			if (end != "") {
				writer.WriteStartElement("End");
				if (endColor != null && !endColor.NoColor)
					endColor.WriteXmlAttributes(writer);
				writer.WriteString(end);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
		
		public SpanNode(string Name)
		{
			name = Name;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			panel = new SpanOptionPanel(this);
		}
		
		
		public override void UpdateNodeText()
		{
			if (name != "") { Text = name; return; }
			
			if (end == "" && stopEOL) {
				Text = begin + " to EOL";
			} else {
				Text = begin + " to " + end;
			}
		}
		
		public bool StopEOL {
			get {
				return stopEOL;
			}
			set {
				stopEOL = value;
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
		
		public EditorHighlightColor BeginColor {
			get {
				return beginColor;
			}
			set {
				beginColor = value;
			}
		}
		
		public EditorHighlightColor EndColor {
			get {
				return endColor;
			}
			set {
				endColor = value;
			}
		}
		
		public string Begin {
			get {
				return begin;
			}
			set {
				begin = value;
			}
		}
		
		public string End {
			get {
				return end;
			}
			set {
				end = value;
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
		
		public string Rule {
			get {
				return rule;
			}
			set {
				rule = value;
			}
		}
		
		public bool NoEscapeSequences {
			get {
				return noEscapeSequences;
			}
			set {
				noEscapeSequences = value;
			}
		}

	}
	
	class SpanOptionPanel : NodeOptionPanel {
		
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.TextBox beginBox;
		private System.Windows.Forms.TextBox endBox;
		private System.Windows.Forms.ComboBox ruleBox;
		private System.Windows.Forms.CheckBox useBegin;
		private System.Windows.Forms.CheckBox useEnd;
		private System.Windows.Forms.Button chgBegin;
		private System.Windows.Forms.Button chgEnd;
		private System.Windows.Forms.Button chgCont;
		private System.Windows.Forms.Label samBegin;
		private System.Windows.Forms.Label samEnd;
		private System.Windows.Forms.Label samCont;
		private System.Windows.Forms.CheckBox noEscBox;
		private System.Windows.Forms.CheckBox stopEolBox;
		
		public SpanOptionPanel(SpanNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Span.xfrm"));
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			beginBox = (TextBox)ControlDictionary["beginBox"];
			endBox   = (TextBox)ControlDictionary["endBox"];
			ruleBox  = (ComboBox)ControlDictionary["ruleBox"];

			useBegin = (CheckBox)ControlDictionary["useBegin"];
			useEnd   = (CheckBox)ControlDictionary["useEnd"];

			chgBegin = (Button)ControlDictionary["chgBegin"];
			chgEnd   = (Button)ControlDictionary["chgEnd"];
			chgCont  = (Button)ControlDictionary["chgCont"];
			
			samBegin = (Label)ControlDictionary["samBegin"];
			samEnd   = (Label)ControlDictionary["samEnd"];
			samCont  = (Label)ControlDictionary["samCont"];

			stopEolBox = (CheckBox)ControlDictionary["stopEolBox"];
			noEscBox   = (CheckBox)ControlDictionary["noEscBox"];

			this.chgBegin.Click += new EventHandler(chgBeginClick);
			this.chgCont.Click  += new EventHandler(chgContClick);
			this.chgEnd.Click   += new EventHandler(chgEndClick);
			
			this.useBegin.CheckedChanged += new EventHandler(CheckedChanged);
			this.useEnd.CheckedChanged += new EventHandler(CheckedChanged);
		}
		
		EditorHighlightColor color;
		EditorHighlightColor beginColor;
		EditorHighlightColor endColor;
		
		public override void StoreSettings()
		{
			SpanNode node = (SpanNode)parent;
			
			node.Name = nameBox.Text;
			node.Begin = beginBox.Text;
			node.End = endBox.Text;
			node.StopEOL = stopEolBox.Checked;
			node.NoEscapeSequences = noEscBox.Checked;
			node.Rule = ruleBox.Text;
			
			node.Color = color;
			
			if (useBegin.Checked) {
				node.BeginColor = beginColor;
			} else {
				node.BeginColor = new EditorHighlightColor(true);
			}
			
			if (useEnd.Checked) {
				node.EndColor 	= endColor;
			} else {
				node.EndColor = new EditorHighlightColor(true);
			}
		}
		
		public override void LoadSettings()
		{
			SpanNode node = (SpanNode)parent;
			
			try {
				ruleBox.Items.Clear();
				foreach(RuleSetNode rn in node.Parent.Parent.Parent.Nodes) { // list rule sets
					if (!rn.IsRoot) ruleBox.Items.Add(rn.Text);
				}
			} catch {}
			
			Properties properties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			////samBegin.Font = samEnd.Font = samCont.Font = ParseFont(properties.Get("DefaultFont", new Font("Courier New", 10).ToString()));
			samBegin.Font = samEnd.Font = samCont.Font = FontContainer.DefaultFont;

			nameBox.Text = node.Name;
			ruleBox.Text = node.Rule;
			beginBox.Text = node.Begin;
			endBox.Text = node.End;
			stopEolBox.Checked = node.StopEOL;
			noEscBox.Checked = node.NoEscapeSequences;
			
			color = node.Color;
			beginColor = node.BeginColor;
			endColor = node.EndColor;
			
			if (beginColor != null) {
				if (!beginColor.NoColor) useBegin.Checked = true;
			}
			if (endColor != null) {
				if (!endColor.NoColor) useEnd.Checked = true;
			}
			
			PreviewUpdate(samBegin, beginColor);
			PreviewUpdate(samEnd, endColor);
			PreviewUpdate(samCont, color);
			CheckedChanged(null, null);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Span.NameEmpty"));
				return false;
			}
			if (beginBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Span.BeginEmpty"));
				return false;
			}
			
			return true;
		}
		
		void chgBeginClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(beginColor)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					beginColor = dlg.Color;
					PreviewUpdate(samBegin, beginColor);
				}
			}
		}
		
		void chgEndClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(endColor)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					endColor = dlg.Color;
					PreviewUpdate(samEnd, endColor);
				}
			}
		}
		
		void chgContClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(samCont, color);
				}
			}
		}
		
		void CheckedChanged(object sender, EventArgs e)
		{
			chgEnd.Enabled = useEnd.Checked;
			chgBegin.Enabled = useBegin.Checked;
		}
	}
}
