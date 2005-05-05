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
	public class Constructor : DefaultMethod
	{
		public Constructor(ModifierEnum m, IRegion region, IRegion bodyRegion, IClass declaringType)
			: base("#ctor", new ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.ReturnType(declaringType.FullyQualifiedName),
			       m, region, bodyRegion, declaringType)
		{
		}
	}
	
	[Serializable]
	public class Destructor : DefaultMethod
	{
		public Destructor(IRegion region, IRegion bodyRegion, IClass declaringType)
			: base("#dtor", null, ModifierEnum.None, region, bodyRegion, declaringType)
		{
		}
	}
	
	[Serializable]
	public class DefaultMethod : AbstractMember, IMethod
	{
		protected IRegion bodyRegion;
		
		List<IParameter> parameters = null;
		
		public override string DocumentationTag {
			get {
				return "M:" + this.FullyQualifiedName;
			}
		}
		
		public virtual IRegion BodyRegion {
			get {
				return bodyRegion;
			}
		}
		
		public virtual List<IParameter> Parameters {
			get {
				if (parameters == null) {
					parameters = new List<IParameter>();
				}
				return parameters;
			}
			set {
				parameters = value;
			}
		}
		
		public virtual bool IsConstructor {
			get {
				return ReturnType == null || Name == "#ctor";
			}
		}
		
		public DefaultMethod(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultMethod(string name, IReturnType type, ModifierEnum m, IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region     = region;
			this.bodyRegion = bodyRegion;
			Modifiers = m;
		}
		
		public override string ToString()
		{
			return String.Format("[AbstractMethod: FullyQualifiedName={0}, ReturnType = {1}, IsConstructor={2}, Modifier={3}]",
			                     FullyQualifiedName,
			                     ReturnType,
			                     IsConstructor,
			                     base.Modifiers);
		}
		
		public virtual int CompareTo(IMethod value)
		{
			int cmp;
			
			cmp = base.CompareTo((IDecoration)value);
			
			if (cmp != 0) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			if (ReturnType != null) {
				cmp = ReturnType.CompareTo(value.ReturnType);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			if (Region != null) {
				cmp = Region.CompareTo(value.Region);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			return DiffUtility.Compare(Parameters, value.Parameters);
		}
		
		int IComparable.CompareTo(object value)
		{
			if (value == null) {
				return 0;
			}
			return CompareTo((IMethod)value);
		}
	}
}
