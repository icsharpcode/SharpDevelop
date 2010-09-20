// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using System;
using ICSharpCode.NRefactory;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of QualifiedNameWithLocation.
	/// </summary>
	public class QualifiedNameWithLocation : IEquatable<QualifiedNameWithLocation> {
		public QualifiedName QualifiedName { get; private set; }
		
		public int Offset { get; private set; }
		
		public string Name {
			get {
				return QualifiedName.Name;
			}
		}
		
		public string Namespace {
			get {
				return QualifiedName.Namespace;
			}
		}
		
		public string Prefix {
			get {
				return QualifiedName.Prefix;
			}
		}

		public string FullXmlName {
			get {
				string name = Prefix;
				
				if (!string.IsNullOrEmpty(name))
					name += ":";
				
				name += Name;				
				return name;
			}
		}
		
		public QualifiedNameWithLocation(string localName, string namespaceName, string prefix, int offset) {
			QualifiedName = new QualifiedName(localName, namespaceName, prefix);
			Offset = offset;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as QualifiedNameWithLocation);
		}
		
		public override int GetHashCode()
		{
			return QualifiedName.GetHashCode() ^ Offset.GetHashCode();
		}
		
		public bool Equals(QualifiedNameWithLocation other)
		{
			if (other == null)
				return false;
			
			return other.QualifiedName == QualifiedName &&
				other.Offset == Offset;
		}
		
		public static bool operator ==(QualifiedNameWithLocation lhs, QualifiedNameWithLocation rhs)
		{
			if ((object)lhs == null && (object)rhs == null)
				return true;
			if ((object)lhs == null)
				return false;
			
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(QualifiedNameWithLocation lhs, QualifiedNameWithLocation rhs)
		{
			return !(lhs == rhs);
		}
		
		public override string ToString()
		{
			return this.QualifiedName + " Offset: " + Offset;
		}
	}
}
