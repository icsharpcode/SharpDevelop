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
	class SpanNode : AbstractNode
	{
		bool        stopEOL;
		EditorHighlightColor color;
		EditorHighlightColor beginColor = null;
		EditorHighlightColor endColor = null;
		string      begin = String.Empty;
		string      end   = String.Empty;
		string      name  = String.Empty;
		string      rule  = String.Empty;
		char        escapeCharacter;
		bool        isBeginSingleWord;
		bool        isEndSingleWord;
		
		public SpanNode(XmlElement el)
		{
			Text = ResNodeName("Span");
			
			panel = new SpanOptionPanel(this);

			if (el == null) return;
			
			color   = new EditorHighlightColor(el);
			
			if (el.Attributes["rule"] != null) {
				rule = el.Attributes["rule"].InnerText;
			}
			
			if (el.Attributes["escapecharacter"] != null) {
				escapeCharacter = el.Attributes["escapecharacter"].Value[0];
			}
			
			name    = el.Attributes["name"].InnerText;
			if (el.HasAttribute("stopateol")) {
				stopEOL = Boolean.Parse(el.Attributes["stopateol"].InnerText);
			} else {
				stopEOL = true;
			}
			XmlElement beginElement = el["Begin"];
			begin = beginElement.InnerText;
			beginColor = new EditorHighlightColor(beginElement);
			if (beginElement.HasAttribute("singleword")) {
				isBeginSingleWord = Boolean.Parse(beginElement.GetAttribute("singleword"));
			}
			
			XmlElement endElement = el["End"];
			if (endElement != null) {
				end  = endElement.InnerText;
				endColor = new EditorHighlightColor(endElement);
				if (endElement.HasAttribute("singleword")) {
					isEndSingleWord = Boolean.Parse(endElement.GetAttribute("singleword"));
				}
			}
			
			UpdateNodeText();
			
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Span");
			writer.WriteAttributeString("name", name);
			if (escapeCharacter != '\0')
				writer.WriteAttributeString("escapecharacter", escapeCharacter.ToString());
			if (rule != "")
				writer.WriteAttributeString("rule", rule);
			writer.WriteAttributeString("stopateol", stopEOL.ToString().ToLowerInvariant());
			color.WriteXmlAttributes(writer);
			
			writer.WriteStartElement("Begin");
			if (isBeginSingleWord) 
				writer.WriteAttributeString("singleword", isBeginSingleWord.ToString().ToLowerInvariant());
			if (beginColor != null && !beginColor.NoColor)
				beginColor.WriteXmlAttributes(writer);
			writer.WriteString(begin);
			writer.WriteEndElement();
			
			if (end != String.Empty) {
				writer.WriteStartElement("End");
				if (isEndSingleWord) 
					writer.WriteAttributeString("singleword", isEndSingleWord.ToString().ToLowerInvariant());
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
			if (name != String.Empty) { Text = name; return; }
			
			if (end == String.Empty && stopEOL) {
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
		
		public bool IsBeginSingleWord {
			get {
				return isBeginSingleWord;
			}
			set {
				isBeginSingleWord = value;
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
		
		public bool IsEndSingleWord {
			get {
				return isEndSingleWord;
			}
			set {
				isEndSingleWord = value;
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
		
		public char EscapeCharacter {
			get { return escapeCharacter; }
			set { escapeCharacter = value; }
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
		private System.Windows.Forms.TextBox escCharTextBox;
		private System.Windows.Forms.CheckBox stopEolBox;
		private System.Windows.Forms.CheckBox beginSingleWordCheckBox;
		private System.Windows.Forms.CheckBox endSingleWordCheckBox;		
		
		public SpanOptionPanel(SpanNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Span.xfrm"));
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			beginBox = (TextBox)ControlDictionary["beginBox"];
			beginBox.TextChanged += BeginTextChanged;
			endBox   = (TextBox)ControlDictionary["endBox"];
			endBox.TextChanged += EndTextChanged;
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
			beginSingleWordCheckBox = (CheckBox)ControlDictionary["beginSingleWordCheckBox"];
			endSingleWordCheckBox = (CheckBox)ControlDictionary["endSingleWordCheckBox"];
			escCharTextBox   = (TextBox)ControlDictionary["escCharTextBox"];

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
			node.IsBeginSingleWord = beginSingleWordCheckBox.Checked;
			node.IsEndSingleWord = endSingleWordCheckBox.Checked;
			node.EscapeCharacter = escCharTextBox.TextLength > 0 ? escCharTextBox.Text[0] : '\0';
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
			
			samBegin.Font = samEnd.Font = samCont.Font = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.FontContainer.DefaultFont;

			nameBox.Text = node.Name;
			ruleBox.Text = node.Rule;
			beginBox.Text = node.Begin;
			endBox.Text = node.End;
			stopEolBox.Checked = node.StopEOL;
			beginSingleWordCheckBox.Checked = node.IsBeginSingleWord;
			endSingleWordCheckBox.Checked = node.IsEndSingleWord;			
			escCharTextBox.Text = (node.EscapeCharacter == '\0') ? "" : node.EscapeCharacter.ToString();
			
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
			BeginTextChanged(null, null);
			EndTextChanged(null, null);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == String.Empty) {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Span.NameEmpty"));
				return false;
			}
			if (beginBox.Text == String.Empty) {
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
		
		void BeginTextChanged(object sender, EventArgs e)
		{
			beginSingleWordCheckBox.Enabled = beginBox.Text.Length > 0; 
		}
		
		void EndTextChanged(object sender, EventArgs e)
		{
			endSingleWordCheckBox.Enabled = endBox.Text.Length > 0; 
		}
	}
}
