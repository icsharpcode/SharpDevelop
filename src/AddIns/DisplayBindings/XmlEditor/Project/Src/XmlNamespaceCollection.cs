// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.XmlEditor
{
	public class XmlNamespaceCollection : Collection<XmlNamespace>
	{
		public XmlNamespaceCollection()
		{
		}
		
		public XmlNamespace[] ToArray()
		{
			List<XmlNamespace> namespaces = new List<XmlNamespace>(this);
			return namespaces.ToArray();
		}
		
		public string GetNamespaceForPrefix(string prefix)
		{
			foreach (XmlNamespace ns in this) {
				if (ns.Prefix == prefix) {
					return ns.Name;
				}
			}
			return String.Empty;
		}
		
		public string GetPrefix(string namespaceToMatch)
		{
			foreach (XmlNamespace ns in this) {
				if (ns.Name == namespaceToMatch)  {
					return ns.Prefix;
				}
			}
			return String.Empty;
		}
	}
}
