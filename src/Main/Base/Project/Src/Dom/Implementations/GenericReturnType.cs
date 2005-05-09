// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// GenericReturnType is a reference to a type parameter.
	/// </summary>
	public sealed class GenericReturnType : ProxyReturnType
	{
		ITypeParameter typeParameter;
		
		public ITypeParameter TypeParameter {
			get {
				return typeParameter;
			}
		}
		
		public override bool Equals(object o)
		{
			GenericReturnType rt = o as GenericReturnType;
			if (rt == null) return false;
			return typeParameter.Equals(rt.typeParameter);
		}
		
				public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override int GetHashCode()
		{
			return typeParameter.GetHashCode();
		}
		
		public GenericReturnType(ITypeParameter typeParameter)
		{
			if (typeParameter == null)
				throw new ArgumentNullException("typeParameter");
			this.typeParameter = typeParameter;
		}
		
		public override string FullyQualifiedName {
			get {
				return typeParameter.Name;
			}
		}
		
		public override string Name {
			get {
				return typeParameter.Name;
			}
		}
		
		public override string Namespace {
			get {
				return "";
			}
		}
		
		public override string DotNetName {
			get {
				if (typeParameter.Method != null)
					return "``" + typeParameter.Index;
				else
					return "`" + typeParameter.Index;
			}
		}
		
		public override IReturnType BaseType {
			get {
				return null;
				// return typeParameter.Constraint;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[GenericReturnType: {0}]", typeParameter);
		}
	}
}
