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
	public class DefaultEvent : AbstractMember, IEvent
	{
		protected IRegion          bodyRegion;
		protected EventAttributes  eventAttributes;
		protected IMethod          addMethod;
		protected IMethod          removeMethod;
		protected IMethod          raiseMethod;
		
		public override string DocumentationTag {
			get {
				return "E:" + this.DotNetName;
			}
		}
		
		public virtual IRegion BodyRegion {
			get {
				return bodyRegion;
			}
		}

		public virtual EventAttributes EventAttributes {
			get {
				return eventAttributes;
			}
		}
		
		protected DefaultEvent(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public DefaultEvent(string name, IReturnType type, ModifierEnum m, IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType, name)
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
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
						
			return Region.CompareTo(value.Region);
		}
		
		int IComparable.CompareTo(object value) 
		{
			return CompareTo((IEvent)value);
		}
		
		public virtual IMethod AddMethod {
			get {
				return addMethod;
			}
		}
		
		public virtual IMethod RemoveMethod {
			get {
				return removeMethod;
			}
		}
		
		public virtual IMethod RaiseMethod {
			get {
				return raiseMethod;
			}
		}
		
	}
}
