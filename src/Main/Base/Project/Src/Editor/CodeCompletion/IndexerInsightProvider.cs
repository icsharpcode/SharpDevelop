// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Produces MethodInsightItem instances for showing the insight window on indexer calls.
	/// </summary>
	public class IndexerInsightProvider : MethodInsightProvider
	{
		public override IInsightItem[] ProvideInsight(ExpressionResult expressionResult, ResolveResult result)
		{
			if (result == null)
				return null;
			IReturnType type = result.ResolvedType;
			if (type == null)
				return null;
			return (from p in type.GetProperties()
			        where p.IsIndexer
			        select new MethodInsightItem(p)
			       ).ToArray();
		}
	}
}
