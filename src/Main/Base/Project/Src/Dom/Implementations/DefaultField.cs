// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class DefaultField : AbstractMember, IField
	{
		public override string DocumentationTag {
			get {
				return "F:" + this.FullyQualifiedName;
			}
		}
		
		public DefaultField(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultField(IReturnType type, string name, ModifierEnum m, IRegion region, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.Modifiers = m;
		}
		
		public virtual int CompareTo(IField field)
		{
			int cmp;
			
			cmp = base.CompareTo((IDecoration)field);
			if (cmp != 0) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(field.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(field.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			if (ReturnType != null) {
				cmp = ReturnType.CompareTo(field.ReturnType);
				if (cmp != 0) {
					return cmp;
				}
			}
			if (Region != null) {
				return Region.CompareTo(field.Region);
			}
			return 0;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IField)value);
		}
	}
}
