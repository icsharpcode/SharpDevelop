// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides a set of refactoring <see cref="ContextAction" />s.
	/// </summary>
	public interface IContextActionsProvider
	{
		/// <summary>
		/// Gets actions available for current line of the editor.
		/// </summary>
		Task<IContextAction[]> GetAvailableActionsAsync(EditorContext context, CancellationToken cancellationToken);
		
		/// <summary>
		/// Is this provider enabled by user?
		/// </summary>
		bool IsVisible { get; set; }
	}
}
