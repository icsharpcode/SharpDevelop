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
	/// Remembers which paths the user has expanded.
	/// </summary>
	public class ExpandedPaths
	{
		private Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		public ExpandedPaths()
		{
		}
		
		public bool IsExpanded(string path)
		{
			return expandedNodes.ContainsKey(path) && (expandedNodes[path] == true);
		}
		
		public void SetExpanded(string path)
		{
			expandedNodes[path] = true;
		}
		
		public void SetCollapsed(string path)
		{
			expandedNodes[path] = false;
		}
	}
}
