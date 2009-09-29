// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	sealed class ExpressionSqlWriter
	{
		readonly TextWriter w;
		readonly ParameterExpression callTreeNodeParameter;
		readonly CallTreeNodeSqlNameSet nameSet;
		
		public ExpressionSqlWriter(TextWriter w, CallTreeNodeSqlNameSet nameSet, ParameterExpression callTreeNodeParameter)
		{
			if (w == null)
				throw new ArgumentNullException("w");
			if (nameSet == null)
				throw new ArgumentNullException("nameSet");
			this.w = w;
			this.nameSet = nameSet;
			this.callTreeNodeParameter = callTreeNodeParameter;
		}
		
		static string EscapeString(string str)
		{
			return "'" + str.Replace("'", "''") + "'";
		}
		
		public void Write(Expression expression)
		{
			switch (expression.NodeType) {
				case ExpressionType.MemberAccess:
					WriteMemberAccess((MemberExpression)expression);
					break;
				case ExpressionType.Call:
					WriteMethodCall((MethodCallExpression)expression);
					break;
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
					BinaryExpression binary = (BinaryExpression)expression;
					w.Write('(');
					Write(binary.Left);
					w.Write(' ');
					w.Write(GetOperatorSymbol(expression.NodeType));
					w.Write(' ');
					Write(binary.Right);
					w.Write(')');
					break;
				case ExpressionType.Constant:
					var ce = (ConstantExpression)expression;
					if (ce.Type == typeof(int))
						w.Write((int)ce.Value);
					else if (ce.Type == typeof(string))
						w.Write(EscapeString((string)ce.Value));
					else
						throw new NotSupportedException("constant of type not supported: " + ce.Type.FullName);
					break;
				case ExpressionType.Not:
					w.Write("(NOT ");
					UnaryExpression unary = (UnaryExpression)expression;
					Write(unary.Operand);
					w.Write(")");
					break;
				default:
					throw new NotSupportedException(expression.NodeType.ToString());
			}
		}

		static string GetOperatorSymbol(ExpressionType nodeType)
		{
			switch (nodeType) {
				case ExpressionType.AndAlso:
					return "AND";
				case ExpressionType.OrElse:
					return "OR";
				case ExpressionType.LessThan:
					return "<";
				case ExpressionType.LessThanOrEqual:
					return "<=";
				case ExpressionType.GreaterThan:
					return ">";
				case ExpressionType.GreaterThanOrEqual:
					return ">=";
				case ExpressionType.Equal:
					return "=";
				case ExpressionType.NotEqual:
					return "!=";
				default:
					throw new NotSupportedException(nodeType.ToString());
			}
		}
		
		void WriteMemberAccess(MemberExpression me)
		{
			if (me.Expression == callTreeNodeParameter) {
				if (!nameSet.IsCalls)
					throw new InvalidOperationException("SingleCall references are invalid here");
				if (me.Member == SingleCall.IDField) {
					w.Write("id");
				} else if (me.Member == SingleCall.DataSetIdField) {
					w.Write("datasetid");
				} else if (me.Member == SingleCall.ParentIDField) {
					w.Write("parentid");
				} else {
					throw new NotSupportedException(me.Member.ToString());
				}
			} else if (IsNameMappingOnParameter(me.Expression)) {
				if (me.Member == KnownMembers.NameMapping_ID) {
					w.Write(nameSet.NameID);
				} else if (me.Member == KnownMembers.NameMapping_Name) {
					w.Write("(SELECT name FROM namemapping WHERE namemapping.id = " + nameSet.NameID + ")");
				} else {
					throw new NotSupportedException(me.Member.ToString());
				}
			} else {
				throw new NotSupportedException(me.Member.ToString());
			}
		}
		
		bool IsNameMappingOnParameter(Expression expr)
		{
			if (expr.NodeType == ExpressionType.MemberAccess) {
				var me = (MemberExpression)expr;
				return me.Expression == callTreeNodeParameter && me.Member == KnownMembers.CallTreeNode_NameMapping;
			} else {
				return false;
			}
		}
		
		void WriteMethodCall(MethodCallExpression mc)
		{
			if (mc.Method == KnownMembers.ListOfInt_Contains) {
				List<int> list = (List<int>)((ConstantExpression)mc.Object).Value;
				w.Write('(');
				Write(mc.Arguments[0]);
				w.Write(" IN (");
				for (int i = 0; i < list.Count; i++) {
					if (i > 0)
						w.Write(',');
					w.Write(list[i]);
				}
				w.Write("))");
			} else if (mc.Method == KnownMembers.Like) {
				w.Write("( ");
				Write(mc.Arguments[0]);
				w.Write(" LIKE ");
				Write(mc.Arguments[1]);
				w.Write(" ESCAPE '' )");
			} else if (mc.Method == KnownMembers.Glob) {
				w.Write("( ");
				Write(mc.Arguments[0]);
				w.Write(" GLOB ");
				Write(mc.Arguments[1]);
				w.Write(" )");
			} else {
				throw new NotSupportedException(mc.Method.ToString());
			}
		}
	}
}
