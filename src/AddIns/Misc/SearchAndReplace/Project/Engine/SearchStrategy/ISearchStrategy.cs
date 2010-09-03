// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	/// <summary>
	/// This interface is the basic interface which all
	/// search algorithms must implement.
	/// </summary>
	public interface ISearchStrategy
	{
		/// <remarks>
		/// Only with a call to this method the search strategy must
		/// update their pattern information. This method will be called
		/// before the FindNext function.
		/// The method might show a message box to the user if the pattern is invalid.
		/// </remarks>
		bool CompilePattern(IProgressMonitor monitor);
		
		/// <remarks>
		/// The find next method should search the next occurrence of the
		/// compiled pattern in the text using the textIterator and options.
		/// </remarks>
		SearchResultMatch FindNext(ITextIterator textIterator);
		
		/// <summary>
		/// Find only in the specified range.
		/// </summary>
		SearchResultMatch FindNext(ITextIterator textIterator, int offset, int length);
	}
}
