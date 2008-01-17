// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

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
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public MemberInfo MemberInfo {
			get { return memberInfo; }
		}
		
		public Expression[] Arguments {
			get { return arguments; }
		}
		
		public MemberReferenceExpression(Expression targetObject, MemberInfo memberInfo, Expression[] arguments)
		{
			if (memberInfo == null) throw new ArgumentNullException("memberInfo");
			
			this.targetObject = targetObject;
			this.memberInfo = memberInfo;
			this.arguments = arguments ?? new Expression[0];
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
				sb.Append(this.CodeTail);
				return sb.ToString();
			}
		}
		
		public override string CodeTail {
			get {
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
	}
}
