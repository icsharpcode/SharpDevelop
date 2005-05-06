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
	/// SpecificReturnType is a reference to class where the type parameters are specified.
	/// Example: List&lt;string&gt;
	/// </summary>
	public sealed class SpecificReturnType : ProxyReturnType
	{
		List<IReturnType> typeParameters;
		IReturnType baseType;
		
		public SpecificReturnType(IReturnType baseType, List<IReturnType> typeParameters)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			if (typeParameters == null)
				throw new ArgumentNullException("typeParameter");
			this.typeParameters = typeParameters;
			this.baseType = baseType;
		}
		
		public override bool Equals(object o)
		{
			SpecificReturnType rt = o as SpecificReturnType;
			if (rt == null) return false;
			if (!baseType.Equals(rt.baseType)) return false;
			if (typeParameters.Count != rt.typeParameters.Count) return false;
			for (int i = 0; i < typeParameters.Count; ++i) {
				if (!typeParameters[i].Equals(rt.typeParameters[i])) return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			int code = baseType.GetHashCode();
			foreach (IReturnType t in typeParameters) {
				code ^= t.GetHashCode();
			}
			return code;
		}
		
		public override IReturnType BaseType {
			get {
				return baseType;
			}
		}
		
		bool CheckReturnType(IReturnType t)
		{
			GenericReturnType rt = t as GenericReturnType;
			if (rt == null) return false;
			return rt.TypeParameter.Method == null;
		}
		
		bool CheckParameters(List<IParameter> l)
		{
			foreach (IParameter p in l) {
				if (CheckReturnType(p.ReturnType)) return true;
			}
			return false;
		}
		
		IReturnType TranslateType(IReturnType input)
		{
			GenericReturnType rt = input as GenericReturnType;
			if (rt == null) return input;
			if (rt.TypeParameter.Method != null) return input;
			if (rt.TypeParameter.Index >= typeParameters.Count) return input;
			return typeParameters[rt.TypeParameter.Index];
		}
		
		public override List<IMethod> GetMethods()
		{
			List<IMethod> l = baseType.GetMethods();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters)) {
					l[i] = (IMethod)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
					for (int j = 0; j < l[i].Parameters.Count; ++j) {
						l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
					}
				}
			}
			return l;
		}
		
		public override List<IProperty> GetProperties()
		{
			List<IProperty> l = baseType.GetProperties();
			return l;
		}
		
		public override List<IField> GetFields()
		{
			List<IField> l = baseType.GetFields();
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			List<IEvent> l = baseType.GetEvents();
			return l;
		}
		
		public override List<IIndexer> GetIndexers()
		{
			List<IIndexer> l = baseType.GetIndexers();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters)) {
					l[i] = (IIndexer)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
					for (int j = 0; j < l[i].Parameters.Count; ++j) {
						l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
					}
				}
			}
			return l;
		}
		
		public override string ToString()
		{
			return String.Format("[SpecificReturnType: {0}<{1}>]", baseType, typeParameters);
		}
	}
}
