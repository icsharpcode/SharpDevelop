// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public QualifiedName(string name, XmlNamespace ns)
			: this(name, ns.Name, ns.Prefix)
		{
		}
		
		public QualifiedName(string name, string namespaceUri, string prefix)
		{
			xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			this.prefix = prefix;
		}
		
		public static bool operator ==(QualifiedName lhs, QualifiedName rhs)
		{
			object lhsObject = (object)lhs;
			object rhsObject = (object)rhs;
			if ((lhsObject != null) && (rhsObject != null)) {
				return lhs.Equals(rhs);
			} else if ((lhsObject == null) && (rhsObject == null)) {
				return true;
			}		
			return false;
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
			QualifiedName qualifiedName = obj as QualifiedName;
			if (qualifiedName != null) {
				return xmlQualifiedName.Equals(qualifiedName.xmlQualifiedName);
			} else {
				XmlQualifiedName name = obj as XmlQualifiedName;
				if (name != null) {
					return xmlQualifiedName.Equals(name);
				}
			}
			return false;
		}
		
		public override int GetHashCode() 
		{
			return xmlQualifiedName.GetHashCode();
		}
		
		public bool IsEmpty {
			get { return xmlQualifiedName.IsEmpty && String.IsNullOrEmpty(prefix); }
		}
		
		/// <summary>
		/// Gets the namespace of the qualified name.
		/// </summary>
		public string Namespace {
			get { return xmlQualifiedName.Namespace; }
			set { xmlQualifiedName = new XmlQualifiedName(xmlQualifiedName.Name, value); }
		}
		
		public bool HasNamespace {
			get { return xmlQualifiedName.Namespace.Length > 0; }
		}		
		
		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public string Name {
			get { return xmlQualifiedName.Name; }
			set { xmlQualifiedName = new XmlQualifiedName(value, xmlQualifiedName.Namespace); }
		}
		
		/// <summary>
		/// Gets the namespace prefix used.
		/// </summary>
		public string Prefix {
			get { return prefix; }
			set { prefix = value; }
		}
		
		public bool HasPrefix {
			get { return !String.IsNullOrEmpty(prefix); }
		}
		
		public override string ToString()
		{
			string qualifiedName = GetPrefixedName();
			if (HasNamespace) {
				return String.Concat(qualifiedName, " [", xmlQualifiedName.Namespace, "]");
			}
			return qualifiedName;
		}
		
		public string GetPrefixedName()
		{
			if (String.IsNullOrEmpty(prefix)) {
				return xmlQualifiedName.Name;
			}
			return prefix + ":" + xmlQualifiedName.Name;
		}
		
		public static QualifiedName FromString(string name)
		{
			if (name == null) {
				return new QualifiedName();
			}
			
			int index = name.IndexOf(':');
			if (index >= 0) {
				return CreateFromNameWithPrefix(name, index);
			}
			return new QualifiedName(name, String.Empty);
		}
		
		static QualifiedName CreateFromNameWithPrefix(string name, int index)
		{
			string prefix = name.Substring(0, index);
			name = name.Substring(index + 1);
			return new QualifiedName(name, String.Empty, prefix);
		}
	}
}
