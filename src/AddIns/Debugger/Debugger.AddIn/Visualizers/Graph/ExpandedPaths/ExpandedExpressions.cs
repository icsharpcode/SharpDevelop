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
	/// Holds the expand state of fields and properties in <see cref="PositionedGraph">.
	/// </summary>
	public class ExpandedExpressions
	{
		private ExpandedPaths expanded = new ExpandedPaths();
		
		public ExpandedExpressions()
		{
		}
		
		public bool IsExpanded(String expression)
		{
			return expanded.IsExpanded(expression);
		}
		
		public void SetExpanded(String expression)
		{
			expanded.SetExpanded(expression);
		}
		
		public void SetCollapsed(String expression)
		{
			expanded.SetCollapsed(expression);
		}
	}
}
