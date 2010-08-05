// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

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
