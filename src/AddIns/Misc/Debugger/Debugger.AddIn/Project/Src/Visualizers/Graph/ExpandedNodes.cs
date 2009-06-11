// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers which expressions the user has expanded.
	/// </summary>
	public class ExpandedNodes
	{
		private Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		public ExpandedNodes()
		{
		}
		
		public bool IsExpanded(string expression)
		{
			return expandedNodes.ContainsKey(expression) && (expandedNodes[expression] == true);
		}
		
		public void Expand(string expression)
		{
			expandedNodes[expression] = true;
		}
		
		public void Collapse(string expression)
		{
			expandedNodes[expression] = false;
		}
	}
}
