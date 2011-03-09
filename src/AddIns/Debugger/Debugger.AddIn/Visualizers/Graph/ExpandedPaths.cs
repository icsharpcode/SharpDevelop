// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers which paths the user has expanded.
	/// </summary>
	public class ExpandedPaths
	{
		private Dictionary<string, bool> expandedPaths = new Dictionary<string, bool>();
		
		public ExpandedPaths()
		{
		}
		
		public bool IsExpanded(string path)
		{
			return expandedPaths.ContainsKey(path) && (expandedPaths[path] == true);
		}
		
		public void SetExpanded(string path)
		{
			expandedPaths[path] = true;
		}
		
		public void SetCollapsed(string path)
		{
			expandedPaths[path] = false;
		}
	}
}
