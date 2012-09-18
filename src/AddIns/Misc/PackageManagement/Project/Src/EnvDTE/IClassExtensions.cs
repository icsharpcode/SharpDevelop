// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public static class IClassExtensions
	{
		/// <summary>
		/// Returns true if the class fully qualified name matches the name or
		/// any class in the inheritance tree matches the name.
		/// </summary>
		public static bool IsDerivedFrom(this IClass c, string typeName)
		{
			if (c.FullyQualifiedName == typeName) {
				return true;
			}
			
			if (TypeNameMatchesBaseType(c.BaseType, typeName)) {
				return true;
			}
			
			return IsTypeInClassInheritanceTree(c, typeName);
		}
		
		static bool TypeNameMatchesBaseType(IReturnType baseType, string typeName)
		{
			return
				(baseType != null) &&
				(baseType.FullyQualifiedName == typeName);
		}
		
		static bool IsTypeInClassInheritanceTree(IClass c, string typeName)
		{
			return c
				.ClassInheritanceTreeClassesOnly
				.Any(inheritedClass => inheritedClass.FullyQualifiedName == typeName);
		}
	}
}
