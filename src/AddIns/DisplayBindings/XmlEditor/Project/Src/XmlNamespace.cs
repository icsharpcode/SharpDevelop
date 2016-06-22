// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public override bool Equals(object obj)
		{
			XmlNamespace rhs = obj as XmlNamespace;
			if (rhs != null) {
				return (Name == rhs.Name) && (Prefix == rhs.Prefix);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Prefix.GetHashCode();
		}
	}
}
