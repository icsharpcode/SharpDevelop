// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers expressions and content nodes the user has expanded in <see cref="PositionedGraph"></see>.
	/// </summary>
	public class Expanded
	{
		private ExpandedExpressions expandedExpressions = new ExpandedExpressions();
		private ExpandedContentNodes expandedContentNodes = new ExpandedContentNodes();
		
		public ExpandedExpressions Expressions
		{
			get { return this.expandedExpressions; }
		}
		
		public ExpandedContentNodes ContentNodes
		{
			get { return this.expandedContentNodes; }
		}
	}
}
