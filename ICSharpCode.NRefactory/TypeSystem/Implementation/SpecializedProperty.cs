// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IProperty (e.g. after type substitution).
	/// </summary>
	public class SpecializedProperty : DefaultProperty
	{
		readonly IMember memberDefinition;
		IType declaringType;
		
		public SpecializedProperty(IProperty p) : base(p)
		{
			this.memberDefinition = p.MemberDefinition;
			this.declaringType = p.DeclaringType;
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
		/// Performs type substitution in parameter types and in the return type.
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
