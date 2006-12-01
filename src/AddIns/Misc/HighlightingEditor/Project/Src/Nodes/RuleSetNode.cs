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

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	class RuleSetNode : AbstractNode
	{
		char escapeCharacter;
		bool ignoreCase   = false;
		bool isRoot       = false;
		string name       = String.Empty;
		string delimiters = String.Empty;
		string reference  = String.Empty;
		
		KeywordListsNode keywordNode;
		SpansNode spansNode;
		MarkersNode prevMarkerNode;
		MarkersNode nextMarkerNode;
		
		public RuleSetNode(XmlElement el)
		{
			Text = ResNodeName("RuleSet");
			
			panel = new RuleSetOptionPanel(this);
			
			if (el == null) return;
			
			if (el.Attributes["name"] != null) {
				name = el.Attributes["name"].InnerText;
				Text = name;
				isRoot = false;
			}
			
			if (name == "") {
				Text = ResNodeName("RootRuleSet");
				isRoot = true;
			}
			
			if (el.Attributes["escapecharacter"] != null) {
				escapeCharacter = el.Attributes["escapecharacter"].InnerText[0];
			}
			
			if (el.Attributes["reference"] != null) {
				reference = el.Attributes["reference"].InnerText;
			}
			
			if (el.Attributes["ignorecase"] != null) {
				ignoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
			}
			
			if (el["Delimiters"] != null) {
				delimiters = el["Delimiters"].InnerText;
			}
			
			keywordNode = new KeywordListsNode(el);
			spansNode   = new SpansNode(el);
			prevMarkerNode = new MarkersNode(el, true);  // Previous
			nextMarkerNode = new MarkersNode(el, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);
			
		}
		
		public RuleSetNode(string Name, string Delim, string Ref, char escChar, bool noCase)
		{
			name = Name;
			Text = Name;
			delimiters = Delim;
			reference = Ref;
			escapeCharacter = escChar;
			ignoreCase = noCase;
			
			keywordNode = new KeywordListsNode(null);
			spansNode   = new SpansNode(null);
			prevMarkerNode = new MarkersNode(null, true);  // Previous
			nextMarkerNode = new MarkersNode(null, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);
			
			panel = new RuleSetOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "" && !isRoot) {
				Text = name;
			}
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("RuleSet");
			if (!isRoot)
				writer.WriteAttributeString("name", name);
			if (reference != "") {
				writer.WriteAttributeString("reference", reference);
			} else {
				writer.WriteAttributeString("ignorecase", ignoreCase.ToString().ToLowerInvariant());
				if (escapeCharacter != '\0')
					writer.WriteAttributeString("escapecharacter", escapeCharacter.ToString());
				if (delimiters != "")
					writer.WriteElementString("Delimiters", delimiters);
				spansNode.WriteXml(writer);
				prevMarkerNode.WriteXml(writer);
				nextMarkerNode.WriteXml(writer);
				keywordNode.WriteXml(writer);
			}
			writer.WriteEndElement();
		}
		
		public string Delimiters {
			get {
				return delimiters;
			}
			set {
				delimiters = value;
			}
		}
		
		public char EscapeCharacter {
			get { return escapeCharacter; }
			set { escapeCharacter = value; }
		}
		
		public bool IgnoreCase {
			get {
				return ignoreCase;
			}
			set {
				ignoreCase = value;
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
		
		public string Reference {
			get {
				return reference;
			}
			set {
				reference = value;
			}
		}
		
		public bool IsRoot {
			get {
				return isRoot;
			}
		}
		
	}
	
	class RuleSetOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.CheckBox igcaseBox;
		private System.Windows.Forms.TextBox escCharTextBox;
		private System.Windows.Forms.TextBox refBox;
		private System.Windows.Forms.TextBox delimBox;
		private System.Windows.Forms.TextBox nameBox;
		
		public RuleSetOptionPanel(RuleSetNode parent) : base(parent)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.RuleSet.xfrm"));
			
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			refBox   = (TextBox)ControlDictionary["refBox"];
			delimBox = (TextBox)ControlDictionary["delimBox"];
			
			igcaseBox = (CheckBox)ControlDictionary["igcaseBox"];
			escCharTextBox  = (TextBox)ControlDictionary["escCharTextBox"];
		}
		
		public override void StoreSettings()
		{
			RuleSetNode node = (RuleSetNode)parent;
			if (!node.IsRoot) node.Name = nameBox.Text;
			node.Reference = refBox.Text;
			node.Delimiters = delimBox.Text;
			node.EscapeCharacter = (escCharTextBox.TextLength == 0) ? '\0' : escCharTextBox.Text[0];
			node.IgnoreCase = igcaseBox.Checked;
		}
		
		public override void LoadSettings()
		{
			RuleSetNode node = (RuleSetNode)parent;
			
			nameBox.Text = node.Name;
			
			if (node.IsRoot) {
				nameBox.Text = ResourceService.GetString("Dialog.HighlightingEditor.TreeView.RootRuleSet");
				nameBox.Enabled = false;
			}
			
			refBox.Text = node.Reference;
			delimBox.Text = node.Delimiters;
			
			escCharTextBox.Text = (node.EscapeCharacter == '\0') ? "" : node.EscapeCharacter.ToString();
			igcaseBox.Checked = node.IgnoreCase;
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.RuleSet.NameEmpty"));
				return false;
			}
			
			return true;
		}

	}
}
