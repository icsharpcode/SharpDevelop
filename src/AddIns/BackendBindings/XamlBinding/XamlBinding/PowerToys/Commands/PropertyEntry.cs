// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	/// <summary>
	/// Description of PropertyEntry.
	/// </summary>
	public class PropertyEntry
	{		
		public IMember Member { get; private set; }
		public XAttribute Attribute { get; private set; }
		public XElement Element { get; private set; }
		
		public PropertyEntry(XAttribute attribute, IMember member)
		{
			this.Attribute = attribute;
			this.Member = member;
		}
		
		public PropertyEntry(XElement element, IMember member)
		{
			this.Element = element;
			this.Member = member;
		}
		
		public bool Selected { get; set; }
		
		public string PropertyName {
			get { return (Attribute == null) ? Element.Name.LocalName : Attribute.Name.LocalName; }
		}
		
		public string PropertyValue {
			get { return (Attribute == null) ? Element.Value : Attribute.Value; }
		}
	}
}
