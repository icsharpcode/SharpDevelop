// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class MarkupExtensionInfo
	{
		public string Type { get; set; }
		public IList<AttributeValue> PositionalArguments { get; set; }
		public IDictionary<string, AttributeValue> NamedArguments { get; set; }
				
		public MarkupExtensionInfo()
			: this(string.Empty, new List<AttributeValue>(), new Dictionary<string, AttributeValue>())
		{
		}
		
		public MarkupExtensionInfo(string type, IList<AttributeValue> posArgs, IDictionary<string, AttributeValue> namedArgs)
		{
			this.Type = type;
			this.PositionalArguments = posArgs;
			this.NamedArguments = namedArgs;
		}
	}
	
	public class AttributeValue
	{
		string stringValue;
		MarkupExtensionInfo extensionValue;
		
		public bool IsString {
			get { return stringValue != null; }
		}
		
		public string StringValue {
			get { return stringValue; }
		}
		
		public MarkupExtensionInfo ExtensionValue {
			get { return extensionValue; }
		}
		
		public AttributeValue(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			this.stringValue = value;
			this.extensionValue = null;
		}
		
		public AttributeValue(MarkupExtensionInfo value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			this.stringValue = null;
			this.extensionValue = value;
		}
	}
}
