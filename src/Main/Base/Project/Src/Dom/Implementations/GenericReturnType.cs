// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
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
		
		public override IClass GetUnderlyingClass()
		{
			return null;
		}
		
		public override IReturnType BaseType {
			get {
				int count = typeParameter.Constraints.Count;
				if (count == 0)
					return ReflectionReturnType.Object;
				if (count == 1)
					return typeParameter.Constraints[0];
				return new CombinedReturnType(typeParameter.Constraints,
				                              FullyQualifiedName,
				                              Name, Namespace,
				                              DotNetName);
			}
		}
		
		// remove static methods (T.ReferenceEquals() is not possible)
		public override List<IMethod> GetMethods()
		{
			List<IMethod> list = base.GetMethods();
			if (list != null) {
				for (int i = 0; i < list.Count; i++) {
					if (list[i].IsStatic) {
						list.RemoveAt(i--);
					}
				}
				if (typeParameter.HasConstructableContraint) {
					list.Add(new Constructor(ModifierEnum.Public, this));
				}
			}
			return list;
		}
		
		public override string ToString()
		{
			return String.Format("[GenericReturnType: {0}]", typeParameter);
		}
	}
}
