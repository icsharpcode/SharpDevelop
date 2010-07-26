// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type representing resolve errors.
	/// </summary>
	sealed class UnknownType : AbstractType
	{
		public override string Name {
			get { return "?"; }
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override bool Equals(IType other)
		{
			return other is UnknownType;
		}
		
		public override int GetHashCode()
		{
			return 950772036;
		}
	}
}
