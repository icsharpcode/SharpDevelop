// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	public abstract class TypeWithElementType : AbstractType
	{
		protected readonly IType elementType;
		
		protected TypeWithElementType(IType elementType)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			this.elementType = elementType;
		}
		
		public override string Name {
			get { return elementType.Name + NameSuffix; }
		}
		
		public override string Namespace {
			get { return elementType.Namespace; }
		}
		
		public override string FullName {
			get { return elementType.FullName + NameSuffix; }
		}
		
		public override string DotNetName {
			get { return elementType.DotNetName + NameSuffix; }
		}
		
		public abstract string NameSuffix { get; }
		
		public override IType GetElementType()
		{
			return elementType;
		}
	}
}
