// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractMember : AbstractNamedEntity, IMember
	{
		protected IReturnType returnType;
		protected IRegion     region;
		
		public virtual IRegion Region {
			get {
				return region;
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
		
		public AbstractMember(IClass declaringType) : base(declaringType)
		{
		}
	}
}
