// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using Hornung.ResourceToolkit.Resolver;

namespace Hornung.ResourceToolkit.Refactoring
{
	/// <summary>
	/// Describes an object that finds resource references in a text document.
	/// </summary>
	public interface IResourceReferenceFinder
	{
		/// <summary>
		/// Returns the offset of the next possible resource reference in the file
		/// after prevOffset.
		/// Returns -1, if there are no more possible references.
		/// </summary>
		/// <param name="fileName">The name of the file that is currently being searched in.</param>
		/// <param name="fileContent">The text content of the file.</param>
		/// <param name="prevOffset">The offset of the last found reference or -1, if this is the first call in the current file.</param>
		int GetNextPossibleOffset(string fileName, string fileContent, int prevOffset);
		
		/// <summary>
		/// Determines whether the specified ResourceResolveResult describes
		/// a resource that should be included in the search result.
		/// </summary>
		bool IsReferenceToResource(ResourceResolveResult result);
	}
}
