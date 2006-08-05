// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public abstract class AbstractMember : AbstractNamedEntity, IMember
	{
		IReturnType returnType;
		DomRegion     region;
		
		public virtual DomRegion Region {
			get {
				return region;
			}
			set {
				region = value;
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
		
		public AbstractMember(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public abstract IMember Clone();
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
