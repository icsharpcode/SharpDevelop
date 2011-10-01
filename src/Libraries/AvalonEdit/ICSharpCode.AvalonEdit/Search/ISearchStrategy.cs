// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search
{
	/// <summary>
	/// Description of ISearchStrategy.
	/// </summary>
	public interface ISearchStrategy
	{
		IEnumerable<ISearchResult> FindAll(ITextSource document);
	}
	
	public interface ISearchResult : ISegment
	{
		
	}
	
	[Serializable]
	public class SearchPatternException : Exception
	{
		public SearchPatternException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
