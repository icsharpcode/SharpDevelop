// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IEvent (e.g. after type substitution).
	/// </summary>
	public class SpecializedEvent : DefaultEvent
	{
		readonly IMember memberDefinition;
		IType declaringType;
		
		public SpecializedEvent(IEvent e) : base(e)
		{
			this.memberDefinition = e.MemberDefinition;
			this.declaringType = e.DeclaringType;
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
