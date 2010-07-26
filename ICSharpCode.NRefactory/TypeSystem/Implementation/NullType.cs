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
	/// Type of the 'null' literal.
	/// </summary>
	sealed class NullType : AbstractType
	{
		public override string Name {
			get { return "null"; }
		}
		
		public override bool? IsReferenceType {
			get { return true; }
		}
		
		public override bool Equals(IType other)
		{
			return other is NullType;
		}
		
		public override int GetHashCode()
		{
			return 362709548;
		}
	}
}
