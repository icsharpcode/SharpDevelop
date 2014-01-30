// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Performs query optimizations.
	/// See the documentation on SQLiteQueryProvider for the list of optimizations being performed.
	/// 
	/// Nodes returned from 'Visit' can be assumed to be fully optimized (they won't contain any of the patterns
	/// described in the SQLiteQueryProvider optimization documentation).
	/// </summary>
	sealed class OptimizeQueryExpressionVisitor : ExpressionVisitor
	{
		QueryNode Visit(QueryNode queryNode)
		{
			return (QueryNode)base.Visit(queryNode);
		}
		
		protected override Expression VisitExtension(Expression node)
		{
			Filter filter = node as Filter;
			MergeByName mergeByName = node as MergeByName;
			if (filter != null)
				return VisitFilter(filter);
			else if (mergeByName != null)
				return VisitMergeByName(mergeByName);
			else
				return base.VisitExtension(node);
		}
		
		#region Filter optimizations
		QueryNode VisitFilter(Filter filter)
		{
			QueryNode result = OptimizeFilter(filter);
			filter = result as Filter;
			if (filter == null)
				return result;
			
			return ReorderFilter(filter);
		}
		
		static readonly MemberInfo[] SafeMembersForMoveIntoMergeByName = { KnownMembers.CallTreeNode_NameMapping };
		
		/// <summary>
		/// Tries to combine nested filters;
		/// move 'MergeByName' nodes out of filter, if possible
		/// </summary>
		QueryNode ReorderFilter(Filter filter)
		{
			if (filter.Target is Filter) {
				// x.Filter(y).Filter(z) -> x.Filter(y && z)
				Filter innerFilter = (Filter)filter.Target;
				return ReorderFilter(new Filter(innerFilter.Target, innerFilter.Conditions.Concat(filter.Conditions).ToArray()));
			} else if (filter.Target is MergeByName) {
				// x.MergeByName().Filter(<criteria>) -> x.Filter(x, <criteria>).MergeByName() for some safe criterias
				QueryNode innerTarget = filter.Target.Target;
				var conditionsToMoveIntoFilter = filter.Conditions.Where(c => IsConditionSafeVisitor.Test(c, SafeMembersForMoveIntoMergeByName)).ToArray();
				if (conditionsToMoveIntoFilter.Length != 0) {
					MergeByName newTarget = new MergeByName(ReorderFilter(new Filter(innerTarget, conditionsToMoveIntoFilter)));
					var conditionsKeptOutsideFilter = filter.Conditions.Except(conditionsToMoveIntoFilter).ToArray();
					if (conditionsKeptOutsideFilter.Length == 0)
						return newTarget;
					else
						return new Filter(newTarget, conditionsKeptOutsideFilter);
				} else {
					return filter;
				}
			} else {
				return filter;
			}
		}
		
		/// <summary>
		/// Optimizes the filter; but does not try to combine nested filter (etc.)
		/// </summary>
		QueryNode OptimizeFilter(Filter filter)
		{
			QueryNode target = Visit(filter.Target);
			
			List<LambdaExpression> newConditions = new List<LambdaExpression>();
			OptimizeQueryExpressionVisitor optimizer = new OptimizeQueryExpressionVisitor();
			foreach (LambdaExpression expr in filter.Conditions) {
				Expression optimizedExpr = optimizer.Visit(expr.Body);
				if (optimizedExpr.NodeType == ExpressionType.Constant && optimizedExpr.Type == typeof(bool)) {
					bool val = (bool)((ConstantExpression)optimizedExpr).Value;
					if (val)
						continue;
				}
				newConditions.Add(Expression.Lambda(optimizedExpr, expr.Parameters));
			}
			if (newConditions.Count == 0)
				return target;
			else
				return new Filter(target, newConditions.ToArray());
		}
		#endregion
		
		#region MergeByName Optimizations
		QueryNode VisitMergeByName(MergeByName merge)
		{
			// First optimize the Target expression
			QueryNode target = Visit(merge.Target);
			if (target is MergeByName) {
				// x.MergeByName().MergeByName() -> x.MergeByName()
				return target;
			}
			if (target == AllCalls.Instance) {
				// AllCalls.MergeByName() -> AllFunctions
				return new AllFunctions();
			}
			if (target is Filter && target.Target == AllCalls.Instance) {
				// AllCalls.Filter(criteria).MergeByName() -> AllFunctions.Filter(criteria)
				// If criteria accesses no CallTreeNode properties except for NameMapping.
				// Criteria of the form 'start <= c.DataSetID && c.DataSetID <= end' will be converted into AllFunctions(start,end)
				List<LambdaExpression> newConditions = new List<LambdaExpression>();
				bool allIsSafe = true;
				int startDataSetID = -1;
				int endDataSetID = -1;
				foreach (LambdaExpression condition in ((Filter)target).Conditions) {
					if (IsConditionSafeVisitor.Test(condition, SafeMembersForMoveIntoMergeByName)) {
						newConditions.Add(condition);
					} else if (condition.Body.NodeType == ExpressionType.AndAlso && startDataSetID < 0) {
						// try match 'constant <= c.DataSetID && c.DataSetID <= constant', but only if we
						// haven't found it already (startDataSetID is still -1)
						BinaryExpression bin = (BinaryExpression)condition.Body;
						if (bin.Left.NodeType == ExpressionType.LessThanOrEqual && bin.Right.NodeType == ExpressionType.LessThanOrEqual) {
							BinaryExpression left = (BinaryExpression)bin.Left;
							BinaryExpression right = (BinaryExpression)bin.Right;
							if (left.Left.NodeType == ExpressionType.Constant && left.Right.NodeType == ExpressionType.MemberAccess
							    && right.Left.NodeType == ExpressionType.MemberAccess && right.Right.NodeType == ExpressionType.Constant
							    && ((MemberExpression)left.Right).Member == SingleCall.DataSetIdField
							    && ((MemberExpression)right.Left).Member == SingleCall.DataSetIdField)
							{
								startDataSetID = (int)GetConstantValue(left.Left);
								endDataSetID = (int)GetConstantValue(right.Right);
							} else {
								allIsSafe = false;
							}
						} else {
							allIsSafe = false;
						}
					} else {
						allIsSafe = false;
					}
				}
				if (allIsSafe) {
					if (newConditions.Count > 0)
						return new Filter(new AllFunctions(startDataSetID, endDataSetID), newConditions.ToArray());
					else
						return new AllFunctions(startDataSetID, endDataSetID);
				}
			}
			return new MergeByName(target);
		}
		
		static object GetConstantValue(Expression expr)
		{
			return ((ConstantExpression)expr).Value;
		}
		#endregion
		
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			// Optimize List<int>.Contains when the list has 0 or 1 elements
			if (node.Method == KnownMembers.ListOfInt_Contains && node.Object.NodeType == ExpressionType.Constant) {
				List<int> list = (List<int>)((ConstantExpression)node.Object).Value;
//				if (list.Count == 0)
//					return Expression.Constant(false); // we cannot optimize to 'false' because bool constants are not valid
				if (list.Count == 1)
					return Expression.Equal(Visit(node.Arguments[0]), Expression.Constant(list[0]));
			}
			return base.VisitMethodCall(node);
		}
	}
	
	
	sealed class IsConditionSafeVisitor : ExpressionVisitor
	{
		public static bool Test(Expression ex, params MemberInfo[] safeMembers)
		{
			var visitor = new IsConditionSafeVisitor();
			visitor.SafeMembers = safeMembers;
			visitor.Visit(ex);
			return visitor.IsSafe;
		}
		
		MemberInfo[] SafeMembers;
		
		bool IsSafe = true;
		
		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression.NodeType == ExpressionType.Parameter && !SafeMembers.Contains(node.Member))
				IsSafe = false;
			return base.VisitMember(node);
		}
		
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Object != null) {
				if (node.Object.NodeType == ExpressionType.Parameter && !SafeMembers.Contains(node.Method))
					IsSafe = false;
			}
			return base.VisitMethodCall(node);
		}
	}
}
