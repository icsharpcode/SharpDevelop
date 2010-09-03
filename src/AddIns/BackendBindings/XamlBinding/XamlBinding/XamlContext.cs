// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	// Fetch a copy of all relevant values of the AXmlDocument so that XamlContext can be used
	// without having to acquire the reader lock.
	public class ElementWrapper {
		public ElementWrapper(AXmlElement element)
		{
			this.LocalName  = element.LocalName;
			this.Prefix     = element.Prefix;
			this.Namespace  = element.Namespace;
			this.Offset     = element.StartOffset;
			this.Attributes = element.Attributes.Select(attr => new AttributeWrapper(attr)).ToList();
		}
		
		public string LocalName { get; private set; }
		public string Namespace { get; private set; }
		public string Prefix { get; private set; }
		public List<AttributeWrapper> Attributes { get; private set; }
		public int Offset { get; set; }
		
		public string Name {
			get {
				if (string.IsNullOrEmpty(Prefix))
					return LocalName;
				
				return Prefix + ":" + LocalName;
			}
		}
		
		public QualifiedNameWithLocation ToQualifiedName()
		{
			return new QualifiedNameWithLocation(LocalName, Namespace, Prefix, Offset);
		}
		
		public string GetAttributeValue(string localName)
		{
			return GetAttributeValue("", localName);
		}
		
		public string GetAttributeValue(string xmlns, string localName)
		{
			foreach (var attribute in Attributes) {
				if (xmlns == attribute.Namespace && attribute.LocalName == localName)
					return attribute.Value;
			}
			
			return null;
		}
	}
	
	public class AttributeWrapper {
		public string LocalName { get; set; }
		public string Namespace { get; set; }
		public string Prefix { get; set; }
		public string Value { get; set; }
		public int Offset { get; set; }
		
		public AttributeWrapper(AXmlAttribute attribute)
		{
			this.LocalName = attribute.LocalName;
			this.Namespace = attribute.Namespace;
			this.Prefix    = attribute.Prefix;
			this.Value     = attribute.Value;
			this.Offset    = attribute.StartOffset;
		}
		
		public string Name {
			get {
				if (string.IsNullOrEmpty(Prefix))
					return LocalName;
				
				return Prefix + ":" + LocalName;
			}
		}
		
		public QualifiedNameWithLocation ToQualifiedName()
		{
			return new QualifiedNameWithLocation(LocalName, Namespace, Prefix, Offset);
		}
	}
	
	public class XamlContext : ExpressionContext {
		public ElementWrapper ActiveElement { get; set; }
		public ElementWrapper ParentElement { get; set; }
		public List<ElementWrapper> Ancestors { get; set; }
		new public AttributeWrapper Attribute { get; set; }
		public AttributeValue AttributeValue { get; set; }
		public string RawAttributeValue { get; set; }
		public int ValueStartOffset { get; set; }
		public XamlContextDescription Description { get; set; }
		public Dictionary<string, string> XmlnsDefinitions { get; set; }
		public ParseInformation ParseInformation { get; set; }
		public bool InRoot { get; set; }
		public ReadOnlyCollection<string> IgnoredXmlns { get; set; }
		public string XamlNamespacePrefix { get; set; }
		
		public IProjectContent ProjectContent {
			get { 
				if (ParseInformation != null)
					return ParseInformation.CompilationUnit.ProjectContent;
				else
					return null;
			}
		}
		
		public XamlContext() {}
		
		public bool InAttributeValueOrMarkupExtension {
			get { return Description == XamlContextDescription.InMarkupExtension ||
					Description == XamlContextDescription.InAttributeValue; }
		}
		
		public bool InCommentOrCData {
			get { return Description == XamlContextDescription.InComment ||
					Description == XamlContextDescription.InCData; }
		}
		
		public override bool ShowEntry(ICompletionEntry o)
		{
			return true;
		}
	}
	
	public class XamlCompletionContext : XamlContext {
		public XamlCompletionContext() { }
		
		public XamlCompletionContext(XamlContext context)
		{
			this.ActiveElement = context.ActiveElement;
			this.Ancestors = context.Ancestors;
			this.Attribute = context.Attribute;
			this.AttributeValue = context.AttributeValue;
			this.Description = context.Description;
			this.ParentElement = context.ParentElement;
			this.ParseInformation = context.ParseInformation;
			this.RawAttributeValue = context.RawAttributeValue;
			this.ValueStartOffset = context.ValueStartOffset;
			this.XmlnsDefinitions = context.XmlnsDefinitions;
			this.InRoot = context.InRoot;
			this.IgnoredXmlns = context.IgnoredXmlns;
			this.XamlNamespacePrefix = context.XamlNamespacePrefix;
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
		InComment,
		/// <summary>
		/// Inside '&lt;![CDATA[]]&gt;'
		/// </summary>
		InCData
	}
}
