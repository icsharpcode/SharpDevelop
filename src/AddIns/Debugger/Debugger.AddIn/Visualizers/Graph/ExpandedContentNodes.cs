// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
