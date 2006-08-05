// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultEvent : AbstractMember, IEvent
	{
		DomRegion bodyRegion;
		IMethod  addMethod;
		IMethod  removeMethod;
		IMethod  raiseMethod;
		
		public override string DocumentationTag {
			get {
				return "E:" + this.DotNetName;
			}
		}
		
		public virtual DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
			protected set {
				bodyRegion = value;
			}
		}
		
		public override IMember Clone()
		{
			return new DefaultEvent(Name, ReturnType, Modifiers, Region, BodyRegion, DeclaringType);
		}
		
		public DefaultEvent(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultEvent(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
		{
			this.ReturnType = type;
			this.Region     = region;
			this.bodyRegion = bodyRegion;
			Modifiers       = (ModifierEnum)m;
			if (Modifiers == ModifierEnum.None) {
				Modifiers = ModifierEnum.Private;
			}
		}
		
		public virtual int CompareTo(IEvent value)
		{
			int cmp;
			
			if(0 != (cmp = base.CompareTo((IDecoration)value)))
				return cmp;
			
			if (FullyQualifiedName != null) {
				return FullyQualifiedName.CompareTo(value.FullyQualifiedName);
			}
			
			return 0;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IEvent)value);
		}
		
		public virtual IMethod AddMethod {
			get {
				return addMethod;
			}
			protected set {
				addMethod = value;
			}
		}
		
		public virtual IMethod RemoveMethod {
			get {
				return removeMethod;
			}
			protected set {
				removeMethod = value;
			}
		}
		
		public virtual IMethod RaiseMethod {
			get {
				return raiseMethod;
			}
			protected set {
				raiseMethod = value;
			}
		}
	}
}
