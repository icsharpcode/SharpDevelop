// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Search
{
	/// <summary>
	/// Basic interface for search algorithms.
	/// </summary>
	public interface ISearchStrategy
	{
		/// <summary>
		/// Finds all matches for a predicate in the given ITextSource.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		IEnumerable<ISearchResult> FindAll(ITextSource document);
	}
	
	/// <summary>
	/// Represents a search result.
	/// </summary>
	public interface ISearchResult : ISegment
	{
		
	}
	
	/// <inheritdoc/>
	public class SearchPatternException : Exception, ISerializable
	{
		/// <inheritdoc/>
		public SearchPatternException()
		{
		}
		
		/// <inheritdoc/>
		public SearchPatternException(string message) : base(message)
		{
		}
		
		/// <inheritdoc/>
		public SearchPatternException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		/// <inheritdoc/>
		protected SearchPatternException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
