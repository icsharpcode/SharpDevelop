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
		/// Unique identifier for the context actions provider; used to hide context actions
		/// that were disabled by the user.
		/// </summary>
		string ID { get; }
		
		/// <summary>
		/// Gets actions available for current line of the editor.
		/// </summary>
		/// <remarks>
		/// This method gets called on the GUI thread. The method implementation should use
		/// 'Task.Run()' to move the implementation onto a background thread.
		/// </remarks>
		Task<IContextAction[]> GetAvailableActionsAsync(EditorRefactoringContext context, CancellationToken cancellationToken);
		
		/// <summary>
		/// Gets whether the user is allowed to disable this provider.
		/// </summary>
		bool AllowHiding { get; }
		
		/// <summary>
		/// Is this provider enabled by user?
		/// </summary>
		bool IsVisible { get; set; }
	}
}
