// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IField (e.g. after type substitution).
	/// </summary>
	public class SpecializedField : DefaultField
	{
		readonly IMember memberDefinition;
		IType declaringType;
		
		public SpecializedField(IField f) : base(f)
		{
			this.memberDefinition = f.MemberDefinition;
			this.declaringType = f.DeclaringType;
		}
		
		public override IType DeclaringType {
			get { return declaringType; }
		}
		
		public void SetDeclaringType(IType declaringType)
		{
			CheckBeforeMutation();
			this.declaringType = declaringType;
		}
		
		public override IMember MemberDefinition {
			get { return memberDefinition; }
		}
	}
}
