// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using IQToolkit;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	static class KnownMembers
	{
		public static readonly MethodInfo ListOfInt_Contains = MethodOf((List<int> list) => list.Contains(0));
		public static readonly PropertyInfo CallTreeNode_IsUserCode = PropertyOf((CallTreeNode c) => c.IsUserCode);
		public static readonly PropertyInfo CallTreeNode_NameMapping = PropertyOf((CallTreeNode c) => c.NameMapping);
		public static readonly PropertyInfo CallTreeNode_Children = PropertyOf((CallTreeNode c) => c.Children);
		public static readonly PropertyInfo NameMapping_ID = PropertyOf((NameMapping n) => n.Id);
		public static readonly MethodInfo QuerableOfCallTreeNode_Select = MethodOf((IQueryable<CallTreeNode> q) => q.Select(x => x));
		public static readonly MethodInfo QuerableOfCallTreeNode_Where = MethodOf((IQueryable<CallTreeNode> q) => q.Where(x => true));
		
		#region InfoOf Helper Methods
		static MethodInfo MethodOf<T, R>(Expression<Func<T, R>> ex)
		{
			return (MethodInfo)InfoOf(ex);
		}
		
		static PropertyInfo PropertyOf<T, R>(Expression<Func<T, R>> ex)
		{
			return (PropertyInfo)InfoOf(ex);
		}
		
		static MemberInfo InfoOf<T, R>(Expression<Func<T, R>> ex)
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
	}
}
