// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Service for creating decompiled breakpoints.
	/// </summary>
	public class DecompiledBreakpointCreateService : IBreakpointCreateService
	{
		/// <inheritdoc/>
		public BreakpointBookmark Create(object[] parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			
			return Activator.CreateInstance(typeof(DecompiledBreakpointBookmark), parameters) as DecompiledBreakpointBookmark;
		}
	}
}
