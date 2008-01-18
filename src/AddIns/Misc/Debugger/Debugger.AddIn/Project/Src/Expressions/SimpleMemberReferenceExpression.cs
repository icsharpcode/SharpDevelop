// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger;

namespace Debugger.Expressions
{
	public class SimpleMemberReferenceExpression: Expression
	{
		Expression targetObject;
		string member;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public string Member {
			get { return member; }
		}
		
		public SimpleMemberReferenceExpression(Expression targetObject, string member)
		{
			this.targetObject = targetObject;
			this.member = member;
		}
		
		public override string Code {
			get {
				return targetObject.Code + "." + member;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			Value targetValue = targetObject.Evaluate(context);
			return targetValue.GetMemberValue(member);
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (targetObject != null) hashCode += 1000000007 * targetObject.GetHashCode(); 
				if (member != null) hashCode += 1000000009 * member.GetHashCode(); 
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			SimpleMemberReferenceExpression other = obj as SimpleMemberReferenceExpression;
			if (other == null) return false; 
			return object.Equals(this.targetObject, other.targetObject) && this.member == other.member;
		}
		
		#endregion
	}
}
