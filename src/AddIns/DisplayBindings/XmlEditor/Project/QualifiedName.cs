//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using System;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// An <see cref="XmlQualifiedName"/> with the namespace prefix.
	/// </summary>
	/// <remarks>
	/// The namespace prefix active for a namespace is 
	/// needed when an element is inserted via autocompletion. This
	/// class just adds this extra information alongside the 
	/// <see cref="XmlQualifiedName"/>.
	/// </remarks>
	public class QualifiedName
	{
		XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Empty;
		string prefix = String.Empty;
		
		public QualifiedName()
		{
		}
		
		public QualifiedName(string name, string namespaceUri)
			: this(name, namespaceUri, String.Empty)
		{
		}
		
		public QualifiedName(string name, string namespaceUri, string prefix)
		{
			xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			this.prefix = prefix;
		}
		
		public static bool operator ==(QualifiedName lhs, QualifiedName rhs)
		{
			bool equals = false;
			
			if (((object)lhs != null) && ((object)rhs != null)) {
				equals = lhs.Equals(rhs);
			} else if (((object)lhs == null) && ((object)rhs == null)) {
				equals = true;
			}
			
			return equals;
		}
		
		public static bool operator !=(QualifiedName lhs, QualifiedName rhs)
		{
			return !(lhs == rhs);
		}		
		
		/// <summary>
		/// A qualified name is considered equal if the namespace and 
		/// name are the same.  The prefix is ignored.
		/// </summary>
		public override bool Equals(object obj) 
		{
			bool equals = false;
			
			QualifiedName qualifiedName = obj as QualifiedName;
			if (qualifiedName != null) {
				equals = xmlQualifiedName.Equals(qualifiedName.xmlQualifiedName);
			} else {
				XmlQualifiedName name = obj as XmlQualifiedName;
				if (name != null) {
					equals = xmlQualifiedName.Equals(name);
				}
			}
			
			return equals;
		}
		
		public override int GetHashCode() 
		{
			return xmlQualifiedName.GetHashCode();
		}
		
		/// <summary>
		/// Gets the namespace of the qualified name.
		/// </summary>
		public string Namespace {
			get {
				return xmlQualifiedName.Namespace;
			}
			set {
				xmlQualifiedName = new XmlQualifiedName(xmlQualifiedName.Name, value);
			}
		}
		
		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public string Name {
			get {
				return xmlQualifiedName.Name;
			}
			set {
				xmlQualifiedName = new XmlQualifiedName(value, xmlQualifiedName.Namespace);
			}
		}
		
		/// <summary>
		/// Gets the namespace prefix used.
		/// </summary>
		public string Prefix {
			get {
				return prefix;
			}
			set {
				prefix = value;
			}
		}
	}
}
