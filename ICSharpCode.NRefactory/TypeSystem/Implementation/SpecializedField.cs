// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public override string Documentation {
			get { return memberDefinition.Documentation; }
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (memberDefinition != null)
					hashCode += 1000000007 * memberDefinition.GetHashCode();
				if (declaringType != null)
					hashCode += 1000000009 * declaringType.GetHashCode();
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			SpecializedField other = obj as SpecializedField;
			if (other == null)
				return false;
			return object.Equals(this.memberDefinition, other.memberDefinition) && object.Equals(this.declaringType, other.declaringType);
		}
	}
}
