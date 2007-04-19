// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultParameter : IParameter
	{
		public static readonly IList<IParameter> EmptyParameterList = new List<IParameter>().AsReadOnly();
		
		string name;
		string documentation;
		
//		int nameHashCode      = -1;
//		int documentationHash = -1;
		
		IReturnType         returnType;
		ParameterModifiers  modifier;
		DomRegion           region;
		IList<IAttribute>   attributes;
		
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
		
		public DefaultParameter(string name, IReturnType type, DomRegion region) : this(name)
		{
			returnType = type;
			this.region = region;
		}
		
		public DomRegion Region {
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
		public bool IsOptional {
			get {
				return (modifier & ParameterModifiers.Optional) == ParameterModifiers.Optional;
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

		public virtual IList<IAttribute> Attributes {
			get {
				if (attributes == null) {
					attributes = new List<IAttribute>();
				}
				return attributes;
			}
			set {
				attributes = value;
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
		
		public static List<IParameter> Clone(IList<IParameter> l)
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
			
			// two parameters are equal if they have the same return type
			// (they may have different names)
			if (object.Equals(ReturnType, value.ReturnType)) {
				return 0;
			} else {
				// if the parameters are not equal, use the parameter name to provide the ordering
				int r = string.Compare(this.Name, value.Name);
				if (r != 0)
					return r;
				else
					return -1; // but equal names don't make parameters of different return types equal
			}
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo(value as IParameter);
		}
	}
}
