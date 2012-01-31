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
	public struct TypeName : IEquatable<TypeName>
	{
		public readonly string AssemblyName;
		public readonly string Namespace;
		public readonly string Name;
		
		public TypeName(string assemblyName, string @namespace, string name)
		{
			this.AssemblyName = assemblyName;
			this.Namespace = @namespace;
			this.Name = name;
		}
		
		public TypeName(ITypeDefinition typeDefinition)
		{
			this.AssemblyName = typeDefinition.ParentAssembly.AssemblyName;
			this.Namespace = typeDefinition.Namespace;
			this.Name = typeDefinition.Name;
			for (ITypeDefinition parent = typeDefinition.DeclaringTypeDefinition; parent != null; parent = parent.DeclaringTypeDefinition) {
				this.Name = parent.Name + "." + this.Name;
			}
		}
		
		public override string ToString()
		{
			string fullName;
			if (string.IsNullOrEmpty(Namespace))
				fullName = Name;
			else
				fullName = Namespace + "." + Name;
			if (string.IsNullOrEmpty(AssemblyName))
				return fullName;
			else
				return fullName + ", " + AssemblyName;
		}
		
		public override bool Equals(object obj)
		{
			return (obj is TypeName) && Equals((TypeName)obj);
		}
		
		public bool Equals(TypeName other)
		{
			return this.AssemblyName == other.AssemblyName && this.Namespace == other.Namespace && this.Name == other.Name;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (AssemblyName != null)
					hashCode += 1000000007 * AssemblyName.GetHashCode();
				if (Namespace != null)
					hashCode += 1000000009 * Namespace.GetHashCode();
				if (Name != null)
					hashCode += 1000000021 * Name.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(TypeName lhs, TypeName rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(TypeName lhs, TypeName rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}