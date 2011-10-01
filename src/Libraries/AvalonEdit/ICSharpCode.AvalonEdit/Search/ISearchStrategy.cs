// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
	
	public class SearchPatternException : Exception, ISerializable
	{
		public SearchPatternException()
		{
		}

	 	public SearchPatternException(string message) : base(message)
		{
		}

		public SearchPatternException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected SearchPatternException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
