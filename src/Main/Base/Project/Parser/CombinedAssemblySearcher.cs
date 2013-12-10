// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Combines other assembly searchers in a chain.
	/// </summary>
	public class CombinedAssemblySearcher : IAssemblySearcher
	{
		readonly List<IAssemblySearcher> searcherList;
		
		public CombinedAssemblySearcher(params IAssemblySearcher[] searchers)
		{
			searcherList = new List<IAssemblySearcher>();
			
			if ((searchers != null) && (searchers.Length > 0)) {
				searcherList.AddRange(searchers);
			}
		}
		
		public void AddSearcher(IAssemblySearcher searcher)
		{
			if (searcher == null)
				throw new ArgumentNullException("searcher");
			searcherList.Add(searcher);
		}
		
		public void RemoveSearcher(IAssemblySearcher searcher)
		{
			if (searcher == null)
				throw new ArgumentNullException("searcher");
			searcherList.Remove(searcher);
		}
		
		public FileName FindAssembly(DomAssemblyName fullName)
		{
			FileName foundFileName = null;
			// Run through all searchers until we find something
			foreach (var searcher in searcherList) {
				if (searcher != null) {
					foundFileName = searcher.FindAssembly(fullName);
					if (foundFileName != null)
						break;
				}
			}
			
			return foundFileName;
		}
	}
}
