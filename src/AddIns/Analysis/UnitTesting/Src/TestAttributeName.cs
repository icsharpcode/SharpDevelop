// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestAttributeName
	{
		string name = String.Empty;
		string qualifiedName = String.Empty;
		string fullyQualifiedName = String.Empty;
		StringComparer nameComparer;
		
		/// <summary>
		/// Creates a new instance of the NUnit Test Attribute class.
		/// </summary>
		/// <param name="name">The name of the attribute (e.g. Test) not
		/// the full name of the attribute (e.g. TestAttribute).</param>
		/// <param name="nameComparer">The string comparer to use
		/// when comparing attribute names.</param>
		public NUnitTestAttributeName(string name, StringComparer nameComparer)
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
		
		public bool IsEqual(IAttribute attribute)
		{
			return IsEqual(attribute.AttributeType.FullyQualifiedName);
		}
	}
}
