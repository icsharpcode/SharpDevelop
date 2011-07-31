// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.Visualizers.Graph.Layout;
using ICSharpCode.NRefactory.Ast;


namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers which properties the user has expanded in the <see cref="PositionedGraph">.
	/// </summary>
	public class ExpandedExpressions
	{
		private ExpandedPaths expanded = new ExpandedPaths();
		
		public ExpandedExpressions()
		{
		}
		
		public bool IsExpanded(Expression expression)
		{
			return expanded.IsExpanded(expression.PrettyPrint());
		}
		
		public void SetExpanded(Expression expression)
		{
			expanded.SetExpanded(expression.PrettyPrint());
		}
		
		public void SetCollapsed(Expression expression)
		{
			expanded.SetCollapsed(expression.PrettyPrint());
		}
	}
}
