// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
