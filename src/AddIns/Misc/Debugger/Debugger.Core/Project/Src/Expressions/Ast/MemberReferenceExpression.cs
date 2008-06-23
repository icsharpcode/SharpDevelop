// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using Debugger.MetaData;

namespace Debugger.Expressions
{
	/// <summary>
	/// Reference to a member of a class.
	/// Can be field, property or a method.
	/// </summary>
	public class MemberReferenceExpression: Expression
	{
		Expression targetObject;
		MemberInfo memberInfo;
		Expression[] arguments;
		string name;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public MemberInfo MemberInfo {
			get { return memberInfo; }
		}
		
		public Expression[] Arguments {
			get { return arguments; }
		}
		
		public MemberReferenceExpression(Expression targetObject, MemberInfo memberInfo, params Expression[] arguments)
		{
			if (memberInfo == null) throw new ArgumentNullException("memberInfo");
			
			this.targetObject = targetObject;
			this.memberInfo = memberInfo;
			this.arguments = arguments ?? new Expression[0];
			this.name = GetName();
		}
		
		public override string Code {
			get {
				StringBuilder sb = new StringBuilder();
				if (memberInfo.IsStatic) {
					sb.Append(memberInfo.DeclaringType.FullName);
				} else {
					sb.Append(targetObject.Code);
				}
				sb.Append(".");
				sb.Append(name);
				return sb.ToString();
			}
		}
		
		public override string CodeTail {
			get {
				return name;
			}
		}
		
		string GetName()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(memberInfo.Name);
			if (arguments.Length > 0) {
				if (memberInfo is PropertyInfo) {
					sb.Append("[");
				} else {
					sb.Append("(");
				}
				bool isFirst = true;
				foreach(Expression argument in arguments) {
					if (isFirst) {
						isFirst = false;
					} else {
						sb.Append(", ");
					}
					sb.Append(argument.Code);
				}
				if (memberInfo is PropertyInfo) {
					sb.Append("]");
				} else {
					sb.Append(")");
				}
			}
			return sb.ToString();
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			Value targetValue = null;
			if (!memberInfo.IsStatic) {
				if (targetObject == null) {
					throw new GetValueException("Target expected for instance member");
				}
				targetValue = targetObject.Evaluate(context);
			}
			
			List<Value> argumentValues = new List<Value>();
			foreach(Expression argument in arguments) {
				argumentValues.Add(argument.Evaluate(context));
			}
			
			return Value.GetMemberValue(targetValue, memberInfo, argumentValues.ToArray());
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (targetObject != null) hashCode += 1000000007 * targetObject.GetHashCode(); 
				if (memberInfo != null) hashCode += 1000000009 * memberInfo.GetHashCode(); 
				if (arguments != null) hashCode += 1000000021 * GetArrayHashCode(arguments);
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			MemberReferenceExpression other = obj as MemberReferenceExpression;
			if (other == null) return false; 
			return
				object.Equals(this.targetObject, other.targetObject) &&
				object.Equals(this.memberInfo, other.memberInfo) &&
				ArrayEquals(this.arguments, other.arguments);
		}
		
		#endregion
	}
}
