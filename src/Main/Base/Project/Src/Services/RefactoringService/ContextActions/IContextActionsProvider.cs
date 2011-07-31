// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

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
		IEnumerable<IContextAction> GetAvailableActions(EditorContext context);
		
		/// <summary>
		/// Is this provider enabled by user?
		/// </summary>
		bool IsVisible { get; set; }
	}
}
