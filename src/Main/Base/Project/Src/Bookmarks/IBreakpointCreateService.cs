// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Interface for creating bookmarks.
	/// </summary>
	public interface IBreakpointCreateService
	{
		/// <summary>
		/// Creates a new instance of BreakpointBookmark.
		/// </summary>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>A new instance of BreakpointBookmark</returns>
		BreakpointBookmark Create(object[] parameters);
	}
	
	/// <summary>
	/// Service for creating normal breakpoints.
	/// </summary>
	public class BreakpointCreateService : IBreakpointCreateService
	{
		/// <inheritdoc/>
		public BreakpointBookmark Create(object[] parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			
			return Activator.CreateInstance(typeof(BreakpointBookmark), parameters) as BreakpointBookmark;
		}
	}
}
