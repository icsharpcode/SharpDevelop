// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public struct AssemblyQualifiedTypeName : IEquatable<AssemblyQualifiedTypeName>
	{
		public readonly string AssemblyName;
		public readonly FullTypeName TypeName;
		
		public AssemblyQualifiedTypeName(FullTypeName typeName, string assemblyName)
		{
			this.AssemblyName = assemblyName;
			this.TypeName = typeName;
		}
		
		public AssemblyQualifiedTypeName(ITypeDefinition typeDefinition)
		{
			this.AssemblyName = typeDefinition.ParentAssembly.AssemblyName;
			this.TypeName = typeDefinition.FullTypeName;
		}
		
		public override string ToString()
		{
			if (string.IsNullOrEmpty(AssemblyName))
				return TypeName.ToString();
			else
				return TypeName.ToString() + ", " + AssemblyName;
		}
		
		public override bool Equals(object obj)
		{
			return (obj is AssemblyQualifiedTypeName) && Equals((AssemblyQualifiedTypeName)obj);
		}
		
		public bool Equals(AssemblyQualifiedTypeName other)
		{
			return this.AssemblyName == other.AssemblyName && this.TypeName == other.TypeName;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (AssemblyName != null)
					hashCode += 1000000007 * AssemblyName.GetHashCode();
				hashCode += TypeName.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(AssemblyQualifiedTypeName lhs, AssemblyQualifiedTypeName rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(AssemblyQualifiedTypeName lhs, AssemblyQualifiedTypeName rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}