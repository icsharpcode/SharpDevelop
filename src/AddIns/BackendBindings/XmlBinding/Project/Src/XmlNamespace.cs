// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A namespace Uri and a prefix.
	/// </summary>
	public class XmlNamespace
	{
		string prefix = string.Empty;
		string uri = string.Empty;
		
		const string prefixToStringStart = "Prefix [";
		const string uriToStringMiddle = "] Uri [";
		
		public XmlNamespace(string prefix, string uri)
		{
			this.prefix = prefix;
			this.uri = uri;
		}
		
		public string Prefix {
			get {
				return prefix;
			}
		}
		
		public string Uri {
			get {
				return uri;
			}
		}
		
		public override string ToString()
		{
			return string.Concat(prefixToStringStart, prefix, uriToStringMiddle, uri, "]");
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
			return new XmlNamespace(string.Empty, string.Empty);
		}
	}
}
