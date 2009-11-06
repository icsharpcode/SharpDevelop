// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A namespace Uri and a prefix.
	/// </summary>
	public class XmlNamespace
	{
		string prefix = String.Empty;
		string name = String.Empty;
		
		const string prefixToStringStart = "Prefix [";
		const string uriToStringMiddle = "] Uri [";
		
		public XmlNamespace()
		{
		}
		
		public XmlNamespace(string prefix, string name)
		{
			Prefix = prefix;
			Name = name;
		}
		
		public string Prefix {
			get { return prefix; }
			set { 
				prefix = value;
				if (prefix == null) {
					prefix = String.Empty;
				}
			}
		}
		
		public string Name {
			get { return name; }
			set { 
				name = value;
				if (name == null) {
					name = String.Empty;
				}
			}
		}
		
		public bool HasName {
			get { return !String.IsNullOrEmpty(name); }
		}
		
		public override string ToString()
		{
			return String.Concat(prefixToStringStart, prefix, uriToStringMiddle, name, "]");
		}
		
		/// <summary>
		/// Creates an XmlNamespace instance from the given string that is in the
		/// format returned by ToString.
		/// </summary>
		public static XmlNamespace FromString(string namespaceString)
		{
			int prefixIndex = namespaceString.IndexOf(prefixToStringStart, StringComparison.Ordinal);
			if (prefixIndex >= 0) {
				prefixIndex += prefixToStringStart.Length;
				int uriIndex = namespaceString.IndexOf(uriToStringMiddle, prefixIndex, StringComparison.Ordinal);
				if (uriIndex >= 0) {
					string prefix = namespaceString.Substring(prefixIndex, uriIndex - prefixIndex);
					uriIndex += uriToStringMiddle.Length;
					string uri = namespaceString.Substring(uriIndex, namespaceString.Length - (uriIndex + 1));
					return new XmlNamespace(prefix, uri);
				}
			}
			return new XmlNamespace();
		}
	}
}
