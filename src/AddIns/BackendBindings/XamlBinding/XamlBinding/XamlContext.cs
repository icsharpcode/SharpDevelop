// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlContext : ExpressionContext {
		public XmlElementPath Path { get; set; }
		public string AttributeName { get; set; }
		public AttributeValue AttributeValue { get; set; }
		public string RawAttributeValue { get; set; }
		public int ValueStartOffset { get; set; }
		public XamlContextDescription Description { get; set; }
		public Dictionary<string, string> XmlnsDefinitions { get; set; }
		public ParseInformation ParseInformation { get; set; }
		
		public XamlContext() {}
		
		public override bool ShowEntry(object o)
		{
			return true;
		}
		
		public override string ToString()
		{
			return "[XamlContext" + 
				" Description: " + Description +
				" Path: " + Path +
				" AttributeName: " + AttributeName +
				" ValueStartOffset: " + ValueStartOffset +
				" ]";
		}
	}
	
	public class XamlCompletionContext : XamlContext {
		public XamlCompletionContext() { }
		
		public XamlCompletionContext(XamlContext context)
		{
			this.AttributeName = context.AttributeName;
			this.AttributeValue = context.AttributeValue;
			this.Description = context.Description;
			this.ParseInformation = context.ParseInformation;
			this.Path = context.Path;
			this.RawAttributeValue = context.RawAttributeValue;
			this.ValueStartOffset = context.ValueStartOffset;
			this.XmlnsDefinitions = context.XmlnsDefinitions;
		}
		
		public char PressedKey { get; set; }	
		public bool Forced { get; set; }	
		public ITextEditor Editor { get; set; }
	}
	
	public enum XamlContextDescription {
		/// <summary>
		/// Outside any tag
		/// </summary>
		None,
		/// <summary>
		/// After '&lt;'
		/// </summary>
		AtTag,
		/// <summary>
		/// Inside '&lt;TagName &gt;'
		/// </summary>
		InTag,
		/// <summary>
		/// Inside '="Value"'
		/// </summary>
		InAttributeValue,
		/// <summary>
		/// Inside '="{}"'
		/// </summary>
		InMarkupExtension,
		/// <summary>
		/// Inside '&lt;!-- --&gt;'
		/// </summary>
		InComment
	}
}
