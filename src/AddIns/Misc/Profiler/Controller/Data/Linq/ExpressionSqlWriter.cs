// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
		
		public void Write(Expression expression)
		{
			switch (expression.NodeType) {
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
					{
						BinaryExpression binary = (BinaryExpression)expression;
						w.Write('(');
						Write(binary.Left);
						w.Write(' ');
						w.Write(GetOperatorSymbol(expression.NodeType));
						w.Write(' ');
						Write(binary.Right);
						w.Write(')');
						break;
					}
				case ExpressionType.Constant:
					w.Write((int)((ConstantExpression)expression).Value);
					break;
				case ExpressionType.MemberAccess:
					{
						MemberExpression me = (MemberExpression)expression;
						if (me.Expression == callTreeNodeParameter) {
							if (!nameSet.IsCalls)
								throw new InvalidOperationException("SingleCall references are invalid here");
							if (me.Member == SingleCall.IDField) {
								w.Write("id");
								break;
							} else if (me.Member == SingleCall.DataSetIdField) {
								w.Write("datasetid");
								break;
							} else if (me.Member == SingleCall.ParentIDField) {
								w.Write("parentid");
								break;
							} else {
								throw new NotSupportedException(me.Member.ToString());
							}
						} else if (IsNameMappingOnParameter(me.Expression)) {
							if (me.Member == KnownMembers.NameMapping_ID) {
								w.Write(nameSet.NameID);
								break;
							} else {
								throw new NotSupportedException(me.Member.ToString());
							}
						} else {
							throw new NotSupportedException(me.Member.ToString());
						}
					}
				case ExpressionType.Call:
					{
						MethodCallExpression mc = (MethodCallExpression)expression;
						if (mc.Method == KnownMembers.ListOfInt_Contains) {
							List<int> list = (List<int>)((ConstantExpression)mc.Object).Value;
							Write(mc.Arguments[0]);
							w.Write(" IN (");
							for (int i = 0; i < list.Count; i++) {
								if (i > 0)
									w.Write(',');
								w.Write(list[i]);
							}
							w.Write(')');
							break;
						} else {
							throw new NotSupportedException(mc.Method.ToString());
						}
					}
				default:
					throw new NotSupportedException(expression.NodeType.ToString());
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
	}
}
