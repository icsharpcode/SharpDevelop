// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IMethod (e.g. after type substitution).
	/// </summary>
	public class SpecializedMethod : DefaultMethod
	{
		readonly IMember memberDefinition;
		IType declaringType;
		
		public SpecializedMethod(IMethod m) : base(m)
		{
			this.memberDefinition = m.MemberDefinition;
			this.declaringType = m.DeclaringType;
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
		
		/// <summary>
		/// Performts type substitution in parameter types and in the return type.
		/// </summary>
		public void SubstituteTypes(ITypeResolveContext context, TypeVisitor substitution)
		{
			this.ReturnType = this.ReturnType.Resolve(context).AcceptVisitor(substitution);
			var p = this.Parameters;
			for (int i = 0; i < p.Count; i++) {
				IType newType = p[i].Type.Resolve(context).AcceptVisitor(substitution);
				if (newType != p[i].Type) {
					p[i] = new DefaultParameter(p[i]) { Type = newType };
				}
			}
		}
	}
}
