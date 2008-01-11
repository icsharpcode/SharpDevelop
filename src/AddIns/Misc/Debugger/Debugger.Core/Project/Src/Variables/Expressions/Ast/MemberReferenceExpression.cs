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
		
		public MemberReferenceExpression(Expression targetObject, MemberInfo memberInfo)
			:this(targetObject, memberInfo, new Expression[0])
		{
		}
		
		public MemberReferenceExpression(Expression targetObject, MemberInfo memberInfo, Expression[] arguments)
		{
			this.targetObject = targetObject;
			this.memberInfo = memberInfo;
			this.arguments = arguments;
		}
		
		public override string Code {
			get {
				StringBuilder sb = new StringBuilder();
				if (targetObject != null) {
					sb.Append(targetObject.Code);
				} else {
					sb.Append(memberInfo.DeclaringType.FullName);
				}
				sb.Append(".");
				sb.Append(memberInfo.Name);
				if (memberInfo is MethodInfo) {
					sb.Append("(");
					bool isFirst = true;
					foreach(Expression argument in arguments) {
						if (isFirst) {
							isFirst = false;
						} else {
							sb.Append(", ");
						}
						sb.Append(argument.Code);
					}
					sb.Append(")");
				}
				return sb.ToString();
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (memberInfo.IsStatic) {
				if (targetObject != null) {
					throw new ExpressionEvaluateException(this, "Target not expected for static member");
				}
			} else {
				if (targetObject == null) {
					throw new ExpressionEvaluateException(this, "Target expected for instance member");
				}
			}
			List<Value> argumentValues = new List<Value>();
			foreach(Expression argument in arguments) {
				argumentValues.Add(argument.Evaluate(context));
			}
			if (memberInfo.IsStatic) {
				try {
					return Value.GetMemberValue(null, memberInfo, argumentValues.ToArray());
				} catch (CannotGetValueException e) {
					throw new ExpressionEvaluateException(this, e.Message);
				}
			} else {
				Value targetValue = targetObject.Evaluate(context);
				try {
					return targetValue.GetMemberValue(memberInfo, argumentValues.ToArray());
				} catch (CannotGetValueException e) {
					throw new ExpressionEvaluateException(this, e.Message);
				}
			}
		}
	}
}
