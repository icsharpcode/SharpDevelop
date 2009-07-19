// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
		public Location Location { get; private set; }
		
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
		
		public QualifiedNameWithLocation(string localName, string namespaceName, string prefix, int line, int column)
			: this(localName, namespaceName, prefix, new Location(column, line))
		{
		}
		
		public QualifiedNameWithLocation(string localName, string namespaceName, string prefix, Location location) {
			QualifiedName = new QualifiedName(localName, namespaceName, prefix);
			Location = location;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as QualifiedNameWithLocation);
		}
		
		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Location.GetHashCode();
		}
		
		public bool Equals(QualifiedNameWithLocation obj)
		{
			if (obj == null)
				return false;
			
			return obj.QualifiedName == QualifiedName &&
				obj.Location == Location;
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
			return this.QualifiedName + " " + Location;
		}
	}
}
