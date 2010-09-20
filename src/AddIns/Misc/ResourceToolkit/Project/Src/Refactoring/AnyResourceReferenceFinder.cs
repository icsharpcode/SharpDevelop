// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using Hornung.ResourceToolkit.Resolver;

namespace Hornung.ResourceToolkit.Refactoring
{
	/// <summary>
	/// Finds references to resources in a text document.
	/// </summary>
	public class AnyResourceReferenceFinder : IResourceReferenceFinder
	{
		/// <summary>
		/// Returns the offset of the next possible resource reference in the file
		/// after prevOffset.
		/// Returns -1, if there are no more possible references.
		/// </summary>
		/// <param name="fileName">The name of the file that is currently being searched in.</param>
		/// <param name="fileContent">The text content of the file.</param>
		/// <param name="prevOffset">The offset of the last found reference or -1, if this is the first call in the current file.</param>
		public int GetNextPossibleOffset(string fileName, string fileContent, int prevOffset)
		{
			int pos = -1;
			int i;
			
			foreach (string pattern in GetPossiblePatternsForFile(fileName)) {
				if ((i = fileContent.IndexOf(pattern, prevOffset+1, StringComparison.OrdinalIgnoreCase)) >= 0) {
					if (pos == -1 || i < pos) {
						pos = i;
					}
				}
			}
			
			return pos;
		}
		
		/// <summary>
		/// Determines whether the specified ResourceResolveResult describes
		/// a resource that should be included in the search result.
		/// </summary>
		public bool IsReferenceToResource(ResourceResolveResult result)
		{
			return true;
		}
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by any
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public static IEnumerable<string> GetPossiblePatternsForFile(string fileName)
		{
			List<string> patterns = new List<string>();
			foreach (IResourceResolver resolver in ResourceResolverService.Resolvers) {
				if (resolver.SupportsFile(fileName)) {
					foreach (string pattern in resolver.GetPossiblePatternsForFile(fileName)) {
						if (!patterns.Contains(pattern)) {
							patterns.Add(pattern);
						}
					}
				}
			}
			return patterns;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="AnyResourceReferenceFinder"/> class.
		/// </summary>
		public AnyResourceReferenceFinder()
		{
		}
	}
}
