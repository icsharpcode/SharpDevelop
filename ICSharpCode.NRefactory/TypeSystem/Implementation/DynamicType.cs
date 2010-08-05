// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type representing the C# 'dynamic' type.
	/// </summary>
	sealed class DynamicType : AbstractType
	{
		public override string Name {
			get { return "dynamic"; }
		}
		
		public override bool? IsReferenceType {
			get { return true; }
		}
		
		public override bool Equals(IType other)
		{
			return other is DynamicType;
		}
		
		public override int GetHashCode()
		{
			return 31986112;
		}
	}
}
