// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public class TestAttributeName
	{
		string name = String.Empty;
		string qualifiedName = String.Empty;
		string fullyQualifiedName = String.Empty;
		StringComparer nameComparer;
		
		/// <summary>
		/// Creates a new instance of the Test Attribute class.
		/// </summary>
		/// <param name="name">The name of the attribute (e.g. Test) not
		/// the full name of the attribute (e.g. TestAttribute).</param>
		/// <param name="nameComparer">The string comparer to use
		/// when comparing attribute names.</param>
		public TestAttributeName(string name, StringComparer nameComparer)
		{
			this.name = name;
			this.nameComparer = nameComparer;
			qualifiedName = String.Concat(name, "Attribute");
			fullyQualifiedName = String.Concat("NUnit.Framework.", name, "Attribute");
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
	}
}
