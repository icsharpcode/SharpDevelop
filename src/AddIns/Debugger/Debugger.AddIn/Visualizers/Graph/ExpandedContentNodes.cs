// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.AddIn.Visualizers.Graph.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers which content nodes the user has expanded in the <see cref="PositionedGraph">.
	/// </summary>
	public class ExpandedContentNodes
	{
		private ExpandedPaths expanded = new ExpandedPaths();
		
		public ExpandedContentNodes()
		{
		}
		
		public bool IsExpanded(ContentNode contentNode)
		{
			return expanded.IsExpanded(contentNode.FullPath);
		}
		
		public void SetExpanded(ContentNode contentNode)
		{
			expanded.SetExpanded(contentNode.FullPath);
		}
		
		public void SetCollapsed(ContentNode contentNode)
		{
			expanded.SetCollapsed(contentNode.FullPath);
		}
	}
}
