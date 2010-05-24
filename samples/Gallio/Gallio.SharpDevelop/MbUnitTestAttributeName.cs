// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace Gallio.SharpDevelop
{
	public class MbUnitTestAttributeName
	{
		string name = String.Empty;
		string qualifiedName = String.Empty;
		string fullyQualifiedName = String.Empty;
		StringComparer nameComparer;
		
		public MbUnitTestAttributeName(string name, StringComparer nameComparer)
		{
			this.name = name;
			this.nameComparer = nameComparer;
			qualifiedName = String.Concat(name, "Attribute");
			fullyQualifiedName = String.Concat("MbUnit.Framework.", name, "Attribute");
		}
		
		/// <summary>
		/// Determines whether the specified attribute name is a
		/// match to this attribute.
		/// </summary>
		public bool IsEqual(string attributeName)
		{
			if (nameComparer.Equals(attributeName, name) || 
				nameComparer.Equals(attributeName, qualifiedName) ||
				nameComparer.Equals(attributeName, fullyQualifiedName)) {
				return true;
			}
			return false;
		}
		
		public bool IsEqual(IAttribute attribute)
		{
			return IsEqual(attribute.AttributeType.FullyQualifiedName);
		}
	}
}
