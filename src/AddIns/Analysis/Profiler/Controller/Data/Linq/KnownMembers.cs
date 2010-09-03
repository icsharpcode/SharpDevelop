// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	static class KnownMembers
	{
		public static readonly PropertyInfo CallTreeNode_IsUserCode = PropertyOf((CallTreeNode c) => c.IsUserCode);
		public static readonly PropertyInfo CallTreeNode_IsThread = PropertyOf((CallTreeNode c) => c.IsThread);
		public static readonly PropertyInfo CallTreeNode_CpuCyclesSpent = PropertyOf((CallTreeNode c) => c.CpuCyclesSpent);
		public static readonly PropertyInfo CallTreeNode_CallCount = PropertyOf((CallTreeNode c) => c.CallCount);
		public static readonly PropertyInfo CallTreeNode_NameMapping = PropertyOf((CallTreeNode c) => c.NameMapping);
		public static readonly PropertyInfo CallTreeNode_Children = PropertyOf((CallTreeNode c) => c.Children);
		public static readonly PropertyInfo NameMapping_ID = PropertyOf((NameMapping n) => n.Id);
		public static readonly PropertyInfo NameMapping_Name = PropertyOf((NameMapping n) => n.Name);
		
		public static readonly MethodInfo ListOfInt_Contains = MethodOf((List<int> list) => list.Contains(0));
		public static readonly MethodInfo QueryableOfCallTreeNode_Select = MethodOf((IQueryable<CallTreeNode> q) => q.Select(x => x));
		public static readonly MethodInfo QueryableOfCallTreeNode_Where = MethodOf((IQueryable<CallTreeNode> q) => q.Where(x => true));
		public static readonly MethodInfo QueryableOfCallTreeNode_Take = MethodOf((IQueryable<CallTreeNode> q) => q.Take(1));
		public static readonly MethodInfo Queryable_OrderBy = MethodOf((IQueryable<CallTreeNode> q) => q.OrderBy(x => x)).GetGenericMethodDefinition();
		public static readonly MethodInfo Queryable_OrderByDesc = MethodOf((IQueryable<CallTreeNode> q) => q.OrderByDescending(x => x)).GetGenericMethodDefinition();
		public static readonly MethodInfo Queryable_ThenBy = MethodOf((IOrderedQueryable<CallTreeNode> q) => q.ThenBy(x => x)).GetGenericMethodDefinition();
		public static readonly MethodInfo Queryable_ThenBy2 = MethodOf((IOrderedQueryable<CallTreeNode> q) => q.ThenBy(x => x, null)).GetGenericMethodDefinition();
		public static readonly MethodInfo Queryable_ThenByDesc = MethodOf((IOrderedQueryable<CallTreeNode> q) => q.ThenByDescending(x => x)).GetGenericMethodDefinition();
		public static readonly MethodInfo Queryable_ThenByDesc2 = MethodOf((IOrderedQueryable<CallTreeNode> q) => q.ThenByDescending(x => x, null)).GetGenericMethodDefinition();
		
		public static readonly MethodInfo String_StartsWith = MethodOf((string s) => s.StartsWith(s, default(StringComparison)));
		public static readonly MethodInfo String_EndsWith = MethodOf((string s) => s.EndsWith(s, default(StringComparison)));
		
		public static readonly MethodInfo Queryable_Merge = MethodOf((IQueryable<CallTreeNode> q) => q.Merge());
		public static readonly MethodInfo Queryable_MergeByName = MethodOf((IQueryable<CallTreeNode> q) => q.MergeByName());
		public static readonly MethodInfo Queryable_WithQueryLog = MethodOf((IQueryable<CallTreeNode> q) => q.WithQueryLog(null));
		
		public static readonly MethodInfo Like = MethodOf(() => LikeImpl("", ""));
		public static readonly MethodInfo Glob = MethodOf(() => GlobImpl("", ""));
		
		#region InfoOf Helper Methods
		static MethodInfo MethodOf<T, R>(Expression<Func<T, R>> ex)
		{
			return (MethodInfo)InfoOf(ex);
		}
		
		static MethodInfo MethodOf<R>(Expression<Func<R>> ex)
		{
			return (MethodInfo)InfoOf(ex);
		}
		
		static PropertyInfo PropertyOf<T, R>(Expression<Func<T, R>> ex)
		{
			return (PropertyInfo)InfoOf(ex);
		}
		
		static MemberInfo InfoOf(LambdaExpression ex)
		{
			var me = ex.Body as MemberExpression;
			if (me != null)
				return me.Member;
			var mc = ex.Body as MethodCallExpression;
			if (mc != null)
				return mc.Method;
			throw new ArgumentException("InfoOf must be called with a member access or method call.");
		}
		#endregion
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		static bool LikeImpl(string input, string pattern)
		{
			throw new NotImplementedException();
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		static bool GlobImpl(string input, string pattern)
		{
			throw new NotImplementedException();
		}
	}
}
