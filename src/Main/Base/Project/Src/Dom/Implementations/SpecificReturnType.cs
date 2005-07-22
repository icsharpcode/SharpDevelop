// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// SpecificReturnType is a reference to generic class that specifies the type parameters.
	/// When getting the Members, this return type modifies the lists in such a way that the
	/// <see cref="GenericReturnType"/>s are replaced with the return types in the type parameters
	/// collection.
	/// Example: List&lt;string&gt;
	/// </summary>
	public sealed class SpecificReturnType : ProxyReturnType
	{
		// Return types that should be substituted for the generic types
		// If a substitution is unknown (type could not be resolved), the list
		// contains a null entry.
		List<IReturnType> typeParameters;
		IReturnType baseType;
		
		public List<IReturnType> TypeParameters {
			get {
				return typeParameters;
			}
		}
		
		public SpecificReturnType(IReturnType baseType, List<IReturnType> typeParameters)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			if (typeParameters == null)
				throw new ArgumentNullException("typeParameters");
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
			if (t is GenericReturnType) {
				GenericReturnType rt = (GenericReturnType)t;
				return rt.TypeParameter.Method == null;
			} else if (t is ArrayReturnType) {
				return CheckReturnType(((ArrayReturnType)t).ElementType);
			} else if (t is SpecificReturnType) {
				foreach (IReturnType para in ((SpecificReturnType)t).TypeParameters) {
					if (CheckReturnType(para)) return true;
				}
				return false;
			} else {
				return false;
			}
		}
		
		bool CheckParameters(List<IParameter> l)
		{
			foreach (IParameter p in l) {
				if (CheckReturnType(p.ReturnType)) return true;
			}
			return false;
		}
		
		public override string DotNetName {
			get {
				string baseName = baseType.DotNetName;
				int pos = baseName.LastIndexOf('`');
				StringBuilder b;
				if (pos < 0)
					b = new StringBuilder(baseName);
				else
					b = new StringBuilder(baseName, 0, pos, pos + 20);
				b.Append('{');
				for (int i = 0; i < typeParameters.Count; ++i) {
					if (i > 0) b.Append(',');
					b.Append(typeParameters[i].DotNetName);
				}
				b.Append('}');
				return b.ToString();
			}
		}
		
		public static IReturnType TranslateType(IReturnType input, List<IReturnType> typeParameters, bool convertForMethod)
		{
			if (input is GenericReturnType) {
				GenericReturnType rt = (GenericReturnType)input;
				if (convertForMethod ? (rt.TypeParameter.Method != null) : (rt.TypeParameter.Method == null)) {
					if (rt.TypeParameter.Index < typeParameters.Count) {
						return typeParameters[rt.TypeParameter.Index];
					}
				}
			} else if (input is ArrayReturnType) {
				IReturnType e = ((ArrayReturnType)input).ElementType;
				IReturnType t = TranslateType(e, typeParameters, convertForMethod);
				if (e != t && t != null)
					return new ArrayReturnType(t, input.ArrayDimensions);
			} else if (input is SpecificReturnType) {
				SpecificReturnType r = (SpecificReturnType)input;
				List<IReturnType> para = new List<IReturnType>(r.TypeParameters.Count);
				for (int i = 0; i < r.TypeParameters.Count; ++i) {
					para.Add(TranslateType(r.TypeParameters[i], typeParameters, convertForMethod));
				}
				return new SpecificReturnType(r.baseType, para);
			}
			return input;
		}
		
		IReturnType TranslateType(IReturnType input)
		{
			return TranslateType(input, typeParameters, false);
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
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters)) {
					l[i] = (IProperty)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
					for (int j = 0; j < l[i].Parameters.Count; ++j) {
						l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
					}
				}
			}
			return l;
		}
		
		public override List<IField> GetFields()
		{
			List<IField> l = baseType.GetFields();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType)) {
					l[i] = (IField)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
				}
			}
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			List<IEvent> l = baseType.GetEvents();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType)) {
					l[i] = (IEvent)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
				}
			}
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
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[SpecificReturnType: {0}<{1}>]", baseType, typeParameters);
		}
	}
}
