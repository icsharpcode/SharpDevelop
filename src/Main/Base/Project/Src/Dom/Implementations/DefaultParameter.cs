// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class DefaultParameter : System.MarshalByRefObject, IParameter
	{
		string              name;
		string              documentation;
		
//		int nameHashCode      = -1;
//		int documentationHash = -1;
		
		protected IReturnType         returnType;
		protected ParameterModifiers  modifier;
		protected IRegion region;
		List<IAttribute> attributes;
		
		protected DefaultParameter(string name)
		{
			Name = name;
		}
		
		public DefaultParameter(IParameter p)
		{
			this.name = p.Name;
			this.region = p.Region;
			this.modifier = p.Modifiers;
			this.returnType = p.ReturnType;
		}
		
		public DefaultParameter(string name, IReturnType type, IRegion region) : this(name)
		{
			returnType = type;
			this.region = region;
		}
		
		public IRegion Region {
			get {
				return region;
			}
		}
		public bool IsOut {
			get {
				return (modifier & ParameterModifiers.Out) == ParameterModifiers.Out;
			}
		}
		public bool IsRef {
			get {
				return (modifier & ParameterModifiers.Ref) == ParameterModifiers.Ref;
			}
		}
		public bool IsParams {
			get {
				return (modifier & ParameterModifiers.Params) == ParameterModifiers.Params;
			}
		}

		public virtual string Name {
			get {
				return name;
//				return (string)AbstractNamedEntity.fullyQualifiedNames[nameHashCode];
			}
			set {
				name = value;
//				nameHashCode = value.GetHashCode();
//				if (AbstractNamedEntity.fullyQualifiedNames[nameHashCode] == null) {
//					AbstractNamedEntity.fullyQualifiedNames[nameHashCode] = value;
//				}
			}
		}

		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}

		public virtual List<IAttribute> Attributes {
			get {
				if (attributes == null) {
					attributes = new List<IAttribute>();
				}
				return attributes;
			}
		}

		public virtual ParameterModifiers Modifiers {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		
		public string Documentation {
			get {
				return documentation;
//				if (documentationHash == -1) {
//					return String.Empty;
//				}
//				return (string)AbstractDecoration.documentationHashtable[documentationHash];
			}
			set {
				documentation = value;
//				documentationHash = value.GetHashCode();
//				if (AbstractDecoration.documentationHashtable[documentationHash] == null) {
//					AbstractDecoration.documentationHashtable[documentationHash] = value;
//				}
			}
		}
		
		public static List<IParameter> Clone(List<IParameter> l)
		{
			List<IParameter> r = new List<IParameter>(l.Count);
			for (int i = 0; i < l.Count; ++i) {
				r.Add(new DefaultParameter(l[i]));
			}
			return r;
		}
		
		public virtual int CompareTo(IParameter value)
		{
			if (value == null) return -1;
			int cmp;
			
			if(0 != (cmp = ((int)Modifiers - (int)value.Modifiers)))
				return cmp;
			
			if (ReturnType.Equals(value.ReturnType))
				return 0;
			else
				return -1;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo(value as IParameter);
		}
	}
}
