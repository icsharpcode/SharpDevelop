// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultField : AbstractMember, IField
	{
		public override string DocumentationTag {
			get {
				return "F:" + this.DotNetName;
			}
		}
		
		public DefaultField(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultField(IReturnType type, string name, ModifierEnum m, DomRegion region, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region = region;
			this.Modifiers = m;
		}
		
		public override IMember Clone()
		{
			DefaultField field = new DefaultField(ReturnType, Name, Modifiers, Region, DeclaringType);
			field.CopyDocumentationFrom(this);
			return field;
		}
		
		public virtual int CompareTo(IField field)
		{
			int cmp;
			
			cmp = base.CompareTo((IEntity)field);
			if (cmp != 0) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				return FullyQualifiedName.CompareTo(field.FullyQualifiedName);
			}
			return 0;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IField)value);
		}
		
		/// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
		public virtual bool IsLocalVariable {
			get { return false; }
		}
		
		/// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
		public virtual bool IsParameter {
			get { return false; }
		}
		
		public class LocalVariableField : DefaultField
		{
			public override bool IsLocalVariable {
				get { return true; }
			}
			
			public LocalVariableField(IReturnType type, string name, DomRegion region, IClass callingClass)
				: base(type, name, ModifierEnum.None, region, callingClass)
			{
			}
		}
		
		public class ParameterField : DefaultField
		{
			public override bool IsParameter {
				get { return true; }
			}
			
			public ParameterField(IReturnType type, string name, DomRegion region, IClass callingClass)
				: base(type, name, ModifierEnum.None, region, callingClass)
			{
			}
		}
	}
}
